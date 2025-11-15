# University Management System (UMS) - Architecture & Workflow Documentation

## Table of Contents
1. [System Overview](#system-overview)
2. [Architecture](#architecture)
3. [Multi-Tenancy Flow](#multi-tenancy-flow)
4. [Authentication & Authorization Flow](#authentication--authorization-flow)
5. [CQRS Pattern Implementation](#cqrs-pattern-implementation)
6. [Database Architecture](#database-architecture)
7. [API Endpoints](#api-endpoints)
8. [Request Processing Pipeline](#request-processing-pipeline)

---

## System Overview

The University Management System (UMS) is a **multi-tenant SaaS platform** designed for higher education institutions. Each university/school operates as an isolated tenant with its own database, ensuring complete data separation.

### Key Features
- 🏢 **Multi-Tenant Architecture**: Database-per-tenant isolation
- 🔐 **JWT Authentication**: Secure token-based authentication
- 🎯 **Role-Based Authorization**: Student, Faculty, Staff, Admin, SuperAdmin roles
- 📝 **CQRS Pattern**: Command Query Responsibility Segregation
- 🏗️ **Clean Architecture**: Separation of concerns with DDD principles
- ✅ **Input Validation**: FluentValidation pipeline
- 🔒 **Password Security**: BCrypt hashing

---

## Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                             │
│  - Controllers (Auth, Students, Tenants)                    │
│  - Middleware (TenantResolution, ExceptionHandling)         │
│  - Program.cs (Startup Configuration)                       │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                   Application Layer                          │
│  - CQRS Commands & Queries                                  │
│  - Command/Query Handlers                                   │
│  - DTOs (Data Transfer Objects)                             │
│  - FluentValidation Validators                              │
│  - MediatR Pipeline Behaviors                               │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                   Infrastructure Layer                       │
│  - DbContext (Master & Application)                         │
│  - Repositories (Generic & Specialized)                     │
│  - Unit of Work                                             │
│  - Services (JwtService, TenantService)                     │
│  - EF Core Configurations                                   │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                      Core Layer                              │
│  - Entities (Student, Faculty, Course, etc.)                │
│  - Interfaces (IRepository, IJwtService, etc.)              │
│  - Enums & Value Objects                                    │
│  - Domain Exceptions                                        │
│  - Settings (JwtSettings)                                   │
└─────────────────────────────────────────────────────────────┘
```

### Dependency Flow
- **API** → Application → Core
- **Application** → Core
- **Infrastructure** → Application → Core
- **Core** → Independent (no dependencies)

---

## Multi-Tenancy Flow

### Subdomain-Based Tenant Resolution

```mermaid
sequenceDiagram
    participant Client
    participant TenantMiddleware
    participant TenantService
    participant MasterDB
    participant TenantDB
    participant Controller

    Client->>TenantMiddleware: https://harvard.yourdomain.com/api/students
    TenantMiddleware->>TenantMiddleware: Extract subdomain: "harvard"
    
    alt Super Admin Subdomain
        TenantMiddleware->>TenantService: SetTenantContext(null, isSuperAdmin=true)
        TenantMiddleware->>MasterDB: Access Master Database
    else Regular Tenant
        TenantMiddleware->>MasterDB: Query tenant by subdomain
        MasterDB-->>TenantMiddleware: Tenant info + ConnectionString
        TenantMiddleware->>TenantService: SetTenantContext(tenantId, connectionString)
        TenantMiddleware->>TenantDB: Connect to UMS_Tenant_harvard
    end
    
    TenantMiddleware->>Controller: Forward request
    Controller->>TenantDB: Execute business logic
    TenantDB-->>Controller: Return data
    Controller-->>Client: Response
```

### Tenant Resolution Process

1. **Request arrives** at `https://harvard.yourdomain.com/api/students`
2. **TenantResolutionMiddleware** extracts subdomain: `harvard`
3. **Subdomain Check**:
   - If `admin` → Super Admin mode (access Master DB only)
   - If valid tenant → Lookup in Master DB
   - If invalid → 400/404 error
4. **TenantService.SetTenantContext()** stores:
   - `TenantId`: Unique identifier
   - `ConnectionString`: `Server=localhost;Database=UMS_Tenant_harvard;...`
   - `IsSuperAdmin`: Boolean flag
5. **ApplicationDbContext** dynamically created with tenant connection string
6. **All database operations** automatically scoped to tenant's database

### Database-Per-Tenant Architecture

```
Master Database (UMS_Master)
├── Tenants Table
│   ├── Id (Guid)
│   ├── Name: "Harvard University"
│   ├── Subdomain: "harvard"
│   ├── ConnectionString: "...UMS_Tenant_harvard..."
│   ├── IsActive
│   ├── SubscriptionStartDate
│   └── MaxStudents, MaxFaculty, MaxCourses

Tenant Database (UMS_Tenant_harvard)
├── Users (TPH: Student, Faculty, Staff)
├── Courses
├── Sections
├── Enrollments
├── Programs
├── Grades
└── ... (all academic data)
```

---

## Authentication & Authorization Flow

### Registration Flow

```mermaid
sequenceDiagram
    participant Client
    participant AuthController
    participant MediatR
    participant RegisterHandler
    participant UserRepository
    participant UnitOfWork
    participant JwtService
    participant TenantDB

    Client->>AuthController: POST /api/auth/register
    AuthController->>MediatR: Send RegisterCommand
    MediatR->>RegisterHandler: Handle command
    
    RegisterHandler->>UserRepository: EmailExistsAsync(email)
    UserRepository->>TenantDB: Query Users table
    TenantDB-->>UserRepository: Email available
    
    RegisterHandler->>RegisterHandler: Hash password with BCrypt
    RegisterHandler->>RegisterHandler: Create User entity (Student/Faculty/Staff)
    
    RegisterHandler->>UserRepository: AddAsync(user)
    RegisterHandler->>UnitOfWork: SaveChangesAsync()
    UnitOfWork->>TenantDB: INSERT INTO Users
    
    RegisterHandler->>JwtService: GenerateAccessToken(user, tenantId, roles)
    JwtService-->>RegisterHandler: JWT token with claims
    
    RegisterHandler->>JwtService: GenerateRefreshToken()
    JwtService-->>RegisterHandler: Refresh token
    
    RegisterHandler-->>AuthController: AuthResponseDto
    AuthController-->>Client: 201 Created + tokens
```

### Login Flow

```mermaid
sequenceDiagram
    participant Client
    participant AuthController
    participant MediatR
    participant LoginHandler
    participant UserRepository
    participant JwtService
    participant TenantDB

    Client->>AuthController: POST /api/auth/login
    AuthController->>MediatR: Send LoginCommand
    MediatR->>LoginHandler: Handle command
    
    LoginHandler->>UserRepository: GetByEmailAsync(email)
    UserRepository->>TenantDB: Query Users WHERE Email = ?
    TenantDB-->>UserRepository: User entity
    
    LoginHandler->>LoginHandler: BCrypt.Verify(password, user.PasswordHash)
    
    alt Invalid Credentials
        LoginHandler-->>Client: 401 Unauthorized
    else Valid Credentials
        LoginHandler->>JwtService: GenerateAccessToken(user, tenantId, roles)
        JwtService-->>LoginHandler: JWT token
        LoginHandler->>JwtService: GenerateRefreshToken()
        JwtService-->>LoginHandler: Refresh token
        LoginHandler-->>AuthController: AuthResponseDto
        AuthController-->>Client: 200 OK + tokens
    end
```

### JWT Token Structure

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "nameid": "user-guid",
    "email": "student@harvard.edu",
    "name": "John Doe",
    "TenantId": "tenant-guid",
    "UserType": "Student",
    "role": ["Student"],
    "iss": "UMS.API",
    "aud": "UMS.Client",
    "exp": 1234567890,
    "iat": 1234567890
  }
}
```

### Authorization Levels

| Role        | Access Level | Permissions |
|-------------|--------------|-------------|
| SuperAdmin  | Global       | All tenants, tenant management |
| Admin       | Tenant-wide  | All operations within tenant |
| Faculty     | Department   | View students, manage courses, grades |
| Staff       | Limited      | Administrative tasks |
| Student     | Personal     | View own data, enroll in courses |

### Secured Endpoints Example

```csharp
[Authorize]                           // Any authenticated user
public class StudentsController

[Authorize(Roles = "Admin,Faculty")]  // Admin OR Faculty
public async Task<IActionResult> GetAll()

[Authorize(Roles = "Admin")]          // Admin only
public async Task<IActionResult> Create()
```

---

## CQRS Pattern Implementation

### Command Flow (Write Operations)

```mermaid
graph LR
    A[Client Request] --> B[Controller]
    B --> C[MediatR]
    C --> D[Validator]
    D --> E{Valid?}
    E -->|No| F[ValidationException]
    E -->|Yes| G[CommandHandler]
    G --> H[Repository]
    H --> I[UnitOfWork]
    I --> J[Database]
    J --> K[Response]
    K --> A
```

**Example: CreateStudentCommand**
1. Client sends POST /api/students
2. StudentsController receives CreateStudentCommand
3. MediatR sends to ValidationBehavior
4. CreateStudentCommandValidator validates:
   - Email format
   - Required fields
   - Password strength
5. CreateStudentCommandHandler:
   - Checks duplicate email
   - Hashes password with BCrypt
   - Creates Student entity
   - Saves via UnitOfWork
6. Returns student ID

### Query Flow (Read Operations)

```mermaid
graph LR
    A[Client Request] --> B[Controller]
    B --> C[MediatR]
    C --> D[QueryHandler]
    D --> E[Repository]
    E --> F[Database]
    F --> G[AutoMapper]
    G --> H[DTO]
    H --> A
```

**Example: GetAllStudentsQuery**
1. Client sends GET /api/students
2. StudentsController receives GetAllStudentsQuery
3. MediatR sends to GetAllStudentsQueryHandler
4. Handler calls StudentRepository.GetAllAsync()
5. Repository filters: `WHERE !IsDeleted`
6. AutoMapper maps entities to StudentDto
7. Returns list of StudentDto

### MediatR Pipeline Behaviors

```
Request → ValidationBehavior → Handler → Response
             ↓ (if validation fails)
        FluentValidation throws
```

---

## Database Architecture

### Master Database (UMS_Master)

**Purpose**: Tenant registry and super admin operations

```sql
CREATE TABLE Tenants (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Subdomain NVARCHAR(50) NOT NULL UNIQUE,
    ConnectionString NVARCHAR(500) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    -- Subscription Management
    SubscriptionStartDate DATETIME2 NOT NULL,
    SubscriptionEndDate DATETIME2,
    MonthlyFee DECIMAL(18,2),
    
    -- Quotas
    MaxStudents INT,
    MaxFaculty INT,
    MaxCourses INT,
    
    -- Audit Fields
    CreatedAt DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME2,
    UpdatedBy NVARCHAR(100),
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedAt DATETIME2,
    DeletedBy NVARCHAR(100)
);
```

### Tenant Database (UMS_Tenant_{subdomain})

**Purpose**: School-specific data (complete isolation)

#### Users Table (Table-Per-Hierarchy)
```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserType NVARCHAR(50) NOT NULL, -- Discriminator: Student, Faculty, Staff
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    
    -- Student-specific columns
    StudentNumber NVARCHAR(50),
    ProgramId UNIQUEIDENTIFIER,
    EnrollmentDate DATETIME2,
    ExpectedGraduationDate DATETIME2,
    AcademicStatus NVARCHAR(50),
    GPA DECIMAL(3,2),
    TotalCredits INT,
    
    -- Faculty-specific columns
    Department NVARCHAR(100),
    Title NVARCHAR(50), -- Professor, Associate Professor, etc.
    HireDate DATETIME2,
    
    -- Staff-specific columns
    Position NVARCHAR(100),
    
    -- Audit Fields
    CreatedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    ...
);
```

#### Other Key Tables
- **Courses**: Course catalog (code, title, credits, department)
- **Sections**: Course instances (semester, instructor, schedule)
- **Enrollments**: Student-Section relationships
- **Programs**: Degree programs (Bachelor's, Master's, etc.)
- **Grades**: Student performance records

### Soft Delete Pattern

All entities inherit from `BaseEntity`:
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
```

All queries automatically filter: `WHERE !IsDeleted`

---

## API Endpoints

### Authentication Endpoints
```
POST   /api/auth/register    - Register new user (Student/Faculty/Staff)
POST   /api/auth/login       - Login and get JWT tokens
```

### Student Management
```
GET    /api/students         - Get all students (Admin/Faculty) [Requires Auth]
GET    /api/students/{id}    - Get student by ID [Requires Auth]
POST   /api/students         - Create student (Admin only) [Requires Auth]
```

### Tenant Management (Super Admin Only)
```
GET    /api/tenants          - Get all tenants (accessed via admin.yourdomain.com)
POST   /api/tenants          - Create new tenant/school
```

### Request/Response Examples

#### Register Request
```http
POST https://harvard.yourdomain.com/api/auth/register
Content-Type: application/json

{
  "email": "john.doe@harvard.edu",
  "password": "SecureP@ss123",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "userType": "Student",
  "studentId": null,
  "programId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

#### Login Request
```http
POST https://harvard.yourdomain.com/api/auth/login
Content-Type: application/json

{
  "email": "john.doe@harvard.edu",
  "password": "SecureP@ss123"
}
```

#### Auth Response
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "john.doe@harvard.edu",
  "fullName": "John Doe",
  "userType": "Student",
  "roles": ["Student"]
}
```

#### Authenticated Request
```http
GET https://harvard.yourdomain.com/api/students
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Request Processing Pipeline

### Complete Request Lifecycle

```
1. CLIENT REQUEST
   ↓
2. TENANT RESOLUTION MIDDLEWARE
   - Extract subdomain from URL
   - Query Master DB for tenant
   - Set TenantService context
   - Create ApplicationDbContext with tenant connection
   ↓
3. EXCEPTION HANDLING MIDDLEWARE
   - Wraps all subsequent operations
   - Catches domain exceptions
   - Returns formatted error responses
   ↓
4. JWT AUTHENTICATION
   - Validate Bearer token
   - Verify signature, expiration
   - Set ClaimsPrincipal
   ↓
5. AUTHORIZATION
   - Check [Authorize] attribute
   - Validate user roles
   - Verify tenant context matches token
   ↓
6. CONTROLLER
   - Receive request
   - Create Command/Query
   - Send to MediatR
   ↓
7. MEDIATR PIPELINE
   - ValidationBehavior (FluentValidation)
   - CommandHandler or QueryHandler
   ↓
8. HANDLER
   - Business logic
   - Repository calls
   - Data transformations
   ↓
9. REPOSITORY
   - EF Core queries
   - Automatic soft delete filtering
   - Tenant-scoped data
   ↓
10. DATABASE
    - SQL Server operations
    - Tenant-specific database
    ↓
11. RESPONSE
    - AutoMapper to DTO
    - Return to client
```

### Error Handling

#### Standardized Error Response Model

All exceptions are caught by `ExceptionHandlingMiddleware` and transformed into a standardized `ErrorResponse` object defined in `UMS.api/Models/ErrorResponse.cs`:

```csharp
public class ErrorResponse
{
    public int StatusCode { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
    public string TraceId { get; init; } = string.Empty;
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
    public IDictionary<string, string[]>? Errors { get; init; }
}
```

#### Exception Mapping

| Exception Type | HTTP Status | Title | Use Case |
|----------------|-------------|-------|----------|
| `ValidationException` | 400 | "Validation failed" | FluentValidation errors |
| `BadRequestException` | 400 | "Bad request" | Invalid input or business rule violation |
| `InvalidOperationException` | 400 | "Bad request" | Invalid state or operation |
| `UnauthorizedException` | 401 | "Unauthorized" | Authentication failure |
| `UnauthorizedAccessException` | 401 | "Unauthorized" | Missing credentials |
| `NotFoundException` | 404 | "Resource not found" | Entity does not exist |
| `KeyNotFoundException` | 404 | "Resource not found" | Dictionary key missing |
| *All others* | 500 | "Internal server error" | Unexpected errors |

#### Custom Domain Exceptions

```csharp
// Defined in UMS.Core/Exceptions/
throw new NotFoundException($"Student with ID {id} not found");
throw new UnauthorizedException("Invalid credentials");
throw new BadRequestException("Email already exists");
```

#### Example Error Responses

**404 Not Found:**
```json
{
  "statusCode": 404,
  "title": "Resource not found",
  "detail": "Course with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found",
  "traceId": "00-abc123-def456-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

**400 Validation Error:**
```json
{
  "statusCode": 400,
  "title": "Validation failed",
  "detail": "One or more validation errors occurred.",
  "traceId": "00-abc123-def456-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": {
    "Email": ["Email is required."],
    "Password": ["Password must be at least 8 characters."]
  }
}
```

**401 Unauthorized:**
```json
{
  "statusCode": 401,
  "title": "Unauthorized",
  "detail": "Invalid credentials",
  "traceId": "00-abc123-def456-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

**500 Internal Server Error:**
```json
{
  "statusCode": 500,
  "title": "Internal server error",
  "detail": "An unexpected error occurred while processing your request.",
  "traceId": "00-abc123-def456-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

#### Error Logging

All exceptions are logged by `ExceptionHandlingMiddleware` using the injected `ILogger<ExceptionHandlingMiddleware>` before the response is returned to the client:

```csharp
_logger.LogError(ex, "Unhandled exception");
```

---

## Security Considerations

### Password Security
- **BCrypt hashing** with salt (work factor: 10-12)
- Minimum requirements: 8+ chars, uppercase, lowercase, digit, special char
- Never stored in plain text

### JWT Security
- **HS256 algorithm** with 256-bit secret key
- Token expiration: 30 minutes (configurable)
- Refresh token: 7 days (configurable)
- Tokens include tenant context (prevents cross-tenant access)

### Multi-Tenant Isolation
- Complete database separation
- Middleware enforces tenant context
- JWT tokens bound to specific tenant
- No cross-tenant data access possible

### Authorization Layers
1. **Authentication**: Valid JWT required
2. **Role-based**: [Authorize(Roles = "Admin")]
3. **Tenant-scoped**: Data automatically filtered by tenant
4. **Ownership**: Users can only access their own data (implemented in handlers)

---

## Technology Stack

| Layer | Technologies |
|-------|-------------|
| **Framework** | ASP.NET Core 9.0 Web API |
| **Database** | SQL Server with EF Core 9.0 |
| **Authentication** | JWT Bearer with System.IdentityModel.Tokens.Jwt |
| **CQRS** | MediatR 13.1.0 |
| **Validation** | FluentValidation 12.1.0 |
| **Password Hashing** | BCrypt.Net-Next 4.0.3 |
| **Object Mapping** | AutoMapper 13.0.1 |
| **API Documentation** | Swagger/OpenAPI |

---

## Future Enhancements

### Planned Features
- [ ] Course management CRUD
- [ ] Enrollment system with prerequisites
- [ ] Grade management and GPA calculation
- [ ] Academic calendar and schedules
- [ ] Financial management (tuition, payments)
- [ ] Library resource management
- [ ] Attendance tracking
- [ ] Notification system
- [ ] Reporting and analytics

### Technical Improvements
- [ ] Refresh token rotation
- [ ] Redis caching for performance
- [ ] Azure AD B2C integration
- [ ] Multi-factor authentication (MFA)
- [ ] Rate limiting and throttling
- [ ] Comprehensive logging (Serilog)
- [ ] Unit and integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline

---

## Conclusion

The University Management System demonstrates a modern, scalable architecture suitable for multi-tenant SaaS applications. The combination of Clean Architecture, CQRS, and database-per-tenant isolation ensures:

✅ **Security**: Complete data isolation, JWT authentication, role-based authorization  
✅ **Scalability**: Independent tenant databases, horizontal scaling capability  
✅ **Maintainability**: Clear separation of concerns, testable code  
✅ **Extensibility**: Easy to add new features without impacting existing code  

For questions or contributions, please refer to the project repository documentation.

---

*Document Version: 1.0*  
*Last Updated: January 2024*  
*Author: UMS Development Team*
