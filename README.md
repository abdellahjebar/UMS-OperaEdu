# University Management System (UMS) - MVP

A comprehensive University Management System built with **Clean Architecture**, **CQRS pattern**, and **ASP.NET Core 9.0**.

## 🏗️ Architecture

The solution follows **Clean Architecture** principles with clear separation of concerns:

```
UMS.Core          - Domain entities, enums, interfaces
UMS.Application   - Business logic, CQRS handlers, DTOs
UMS.Infrastructure - Data access, EF Core, repositories
UMS.API           - REST API endpoints, middleware
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup

1. **Update Database Connection String** in `UMS.api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "MasterConnection": "Server=(localdb)\\mssqllocaldb;Database=UMS_Master;Trusted_Connection=True;TrustServerCertificate=True;",
  },
  "DatabaseProvider": "SqlServer"
}
```

2. **Run the Application** (migrations and seeding run automatically):
```bash
cd UMS.api
dotnet run
```

The application will:
- Apply database migrations automatically
- Seed the master database with a demo tenant
- Seed tenant databases on first access

> See [DATABASE_SEEDING.md](DATABASE_SEEDING.md) for detailed seeding documentation

3. **Access the API**:
   - Swagger UI: `http://localhost:5263/swagger`
   - Demo tenant API: `http://demo.localhost:5263/api/...`

4. **Default Credentials** (Development Only):
   - Admin: `admin@demouniversity.edu` / `Admin@123`
   - Faculty: `john.doe@demouniversity.edu` / `Faculty@123`
   - Student: `alice.smith@demouniversity.edu` / `Student@123`

## 📦 Technologies Used

### Core Technologies
- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core 9.0** - ORM for database access
- **SQL Server** - Database

### Patterns & Libraries
- **CQRS** with **MediatR** - Command/Query separation
- **Repository Pattern** with **Unit of Work** - Data access abstraction
- **FluentValidation** - Input validation
- **BCrypt.Net** - Password hashing
- **Swagger/OpenAPI** - API documentation

## 🗃️ Database Schema

### Tables Created
- **Users** - Base user table with discriminator for Student/Faculty/Staff
- **Programs** - Academic programs (degrees)
- **Courses** - Course catalog
- **Sections** - Course sections offered in specific terms
- **Enrollments** - Student enrollments in sections

### Key Features
- Soft delete functionality (IsDeleted flag)
- Audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- TPH (Table Per Hierarchy) inheritance for User types

## 📚 API Endpoints

### Error Responses

All API endpoints return standardized error responses following the `ErrorResponse` model:

```json
{
  "statusCode": 404,
  "title": "Resource not found",
  "detail": "Student with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

#### HTTP Status Codes
- **400 Bad Request**: Invalid input or validation errors
- **401 Unauthorized**: Missing or invalid authentication token
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource does not exist
- **500 Internal Server Error**: Unexpected server error

#### Validation Error Response
```json
{
  "statusCode": 400,
  "title": "Validation failed",
  "detail": "One or more validation errors occurred.",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": {
    "Email": ["Email is required.", "Email must be a valid email address."],
    "Password": ["Password must be at least 8 characters."]
  }
}
```

See `UMS.api/Models/ErrorResponse.cs` for the complete contract definition.

### Students

#### Get All Students
```http
GET /api/students
```

#### Get Student by ID
```http
GET /api/students/{id}
```

#### Create Student
```http
POST /api/students
Content-Type: application/json

{
  "email": "john.doe@university.edu",
  "password": "SecurePass123",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "2000-01-15",
  "studentNumber": "S2024001",
  "enrollmentDate": "2024-09-01",
  "programId": "guid-here"
}
```

**Response:**
```json
{
  "id": "generated-guid"
}
```

## 🧪 Testing with Swagger

1. Run the application
2. Navigate to `http://localhost:5263/swagger`
3. Try the Student endpoints:
   - First create a student using POST
   - Then retrieve it using GET by ID
   - List all students with GET all

## 🎯 What's Implemented (MVP Phase 1)

✅ **Infrastructure Layer**
- Repository Pattern implementation
- Unit of Work pattern
- EF Core configurations
- Database migrations

✅ **Application Layer**
- CQRS pattern with MediatR
- Student management (Create, GetById, GetAll)
- FluentValidation for commands
- DTOs for data transfer

✅ **API Layer**
- REST API endpoints
- Global exception handling middleware
- Swagger/OpenAPI documentation
- Dependency injection setup

## 📋 Next Steps for Complete MVP

### Phase 2: Core Features
- [ ] Course management CQRS handlers
- [ ] Enrollment management (enroll, withdraw)
- [ ] Program management
- [ ] Department management

### Phase 3: Authentication & Authorization
- [ ] JWT authentication implementation
- [ ] User login/register endpoints
- [ ] Role-based authorization (Student, Faculty, Admin)
- [ ] Password reset functionality

### Phase 4: Advanced Features
- [ ] Grade management
- [ ] Attendance tracking
- [ ] Schedule/timetable management
- [ ] Faculty assignment to sections

### Phase 5: Reporting & Analytics
- [ ] Student transcript generation
- [ ] Course enrollment reports
- [ ] GPA calculation
- [ ] Academic standing reports

### Phase 6: Quality & Testing
- [ ] Unit tests for business logic
- [ ] Integration tests for API
- [ ] Logging implementation (Serilog)
- [ ] Performance optimization

## 🏛️ Domain Entities

### Student
- Personal information (name, email, phone, DOB)
- Academic info (student number, GPA, credits)
- Program enrollment
- Academic status

### Course
- Code, name, description
- Credit hours
- Department association

### Section
- Course offering in specific term/year
- Instructor assignment
- Capacity management
- Schedule (start/end dates)

### Enrollment
- Links students to sections
- Enrollment status tracking
- Grade management

## 🔧 Configuration

### Application Settings (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UniversityDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## 🐛 Troubleshooting

### Database Connection Issues
- Ensure SQL Server is running
- Verify connection string in `appsettings.json`
- Check Windows Authentication is enabled if using Trusted_Connection

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove --project UMS.Infrastructure --startup-project UMS.api

# Create new migration
dotnet ef migrations add MigrationName --project UMS.Infrastructure --startup-project UMS.api

# Update database
dotnet ef database update --project UMS.Infrastructure --startup-project UMS.api
```

## 📝 Development Guidelines

### Adding a New Entity
1. Create entity in `UMS.Core/Entities`
2. Create configuration in `UMS.Infrastructure/Configurations`
3. Add DbSet to `ApplicationDbContext`
4. Create migration
5. Update database

### Adding a New Feature
1. Create DTOs in `UMS.Application/DTOs`
2. Create Command/Query in `UMS.Application/Features`
3. Create Handler for the Command/Query
4. Add Validator (if Command)
5. Create Controller endpoint in `UMS.API`

## 📖 Documentation

- **[DATABASE_SEEDING.md](DATABASE_SEEDING.md)** - Database initialization and seeding guide
- **[WORKFLOW_DOCUMENTATION.md](WORKFLOW_DOCUMENTATION.md)** - Error handling and middleware documentation
- **[API_EXAMPLES.md](API_EXAMPLES.md)** - API request/response examples
- **[DEVELOPMENT.md](DEVELOPMENT.md)** - Development guidelines and best practices
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Deployment instructions

## 🤝 Contributing

This is an MVP project. Future enhancements should maintain:
- Clean Architecture principles
- CQRS pattern for business logic
- Comprehensive validation
- API documentation

## 📄 License

This project is for educational purposes.

---

**Current Status:** MVP Phase 1 Complete ✅
- Database schema created
- Repository pattern implemented
- Student management operational
- API endpoints functional
- Swagger documentation available

**API Running At:** `http://localhost:5263`
**Swagger UI:** `http://localhost:5263/swagger`
