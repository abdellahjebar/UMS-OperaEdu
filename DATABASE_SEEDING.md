# Database Seeding Documentation

## Overview

The UMS application implements automatic database initialization and seeding for both master and tenant databases. This ensures that new deployments and fresh database instances start with required baseline data.

## Architecture

### Components

1. **MasterDbSeeder** - Seeds the master database with initial tenant configuration
2. **TenantDbSeeder** - Seeds tenant databases with sample academic data
3. **DatabaseExtensions** - Provides initialization methods called at application startup
4. **DatabaseInitializationService** - Handles tenant database creation and seeding on-demand

### Seeding Workflow

```
Application Startup
    └─> InitializeDatabasesAsync()
        └─> Apply master DB migrations
        └─> Seed master database (if empty)
        └─> Tenant DBs are seeded on-demand when accessed
```

## Master Database Seeding

### Location
`UMS.Infrastructure/Persistance/Seeders/MasterDbSeeder.cs`

### Data Seeded

#### Demo Tenant
- **Name**: Demo University
- **Subdomain**: `demo`
- **Connection String**: Auto-generated for LocalDB/PostgreSQL
- **Subscription**: Active for 1 year from creation
- **Annual Fee**: $10,000
- **Admin Email**: admin@demouniversity.edu

### Connection String Generation

The seeder generates tenant-specific connection strings based on the configured database provider:

**SQL Server (LocalDB)**:
```
Server=(localdb)\mssqllocaldb;Database=UMS_Tenant_{subdomain};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

**PostgreSQL**:
```
Host=localhost;Database=ums_tenant_{subdomain};Username=postgres;Password=your_password
```

> **Note**: In production, connection strings should be stored securely (Azure Key Vault, AWS Secrets Manager, etc.)

## Tenant Database Seeding

### Location
`UMS.Infrastructure/Persistance/Seeders/TenantDbSeeder.cs`

### Data Seeded

#### 1. Users

**Admin User (Staff)**
- Name: System Administrator
- Email: admin@demouniversity.edu
- Employee Number: A2024001
- Job Title: System Administrator
- User Type: Admin
- Password Hash: BCrypt hash (plaintext: "Admin@123")

**Faculty User**
- Name: John Doe
- Email: john.doe@demouniversity.edu
- Employee Number: F2024001
- Title: Associate Professor
- Department: Computer Science
- Office: Building A, Room 201
- Office Hours: Mon/Wed 2-4 PM
- User Type: Faculty
- Password Hash: BCrypt hash (plaintext: "Faculty@123")

**Student Users** (2 students)
1. Alice Smith
   - Email: alice.smith@demouniversity.edu
   - Student Number: S2024001
   - Program: Computer Science (BS)
   - GPA: 3.5
   - Credits: 30
   - User Type: Student
   - Password Hash: BCrypt hash (plaintext: "Student@123")

2. Bob Jones
   - Email: bob.jones@demouniversity.edu
   - Student Number: S2024002
   - Program: Computer Science (BS)
   - GPA: 3.2
   - Credits: 24
   - User Type: Student
   - Password Hash: BCrypt hash (plaintext: "Student@123")

#### 2. Academic Programs

**Bachelor of Science in Computer Science (CS-BS)**
- Department: Computer Science
- Degree Type: Bachelor
- Duration: 4 years
- Required Credits: 120

**Bachelor of Science in Electrical Engineering (EE-BS)**
- Department: Electrical Engineering
- Degree Type: Bachelor
- Duration: 4 years
- Required Credits: 128

#### 3. Courses

**CS101 - Introduction to Programming**
- Credits: 3
- Description: Fundamentals of programming using modern languages

**CS201 - Data Structures**
- Credits: 4
- Description: Advanced data structures and algorithms

**CS301 - Database Systems**
- Credits: 3
- Description: Relational databases, SQL, and database design

#### 4. Sections

**Section CS101-001 (Fall 2024)**
- Instructor: John Doe
- Max Capacity: 30 students
- Term: Fall 2024
- Dates: Sep 1 - Dec 15, 2024

**Section CS201-001 (Fall 2024)**
- Instructor: John Doe
- Max Capacity: 25 students
- Term: Fall 2024
- Dates: Sep 1 - Dec 15, 2024

#### 5. Enrollments

- Alice Smith enrolled in CS101-001 and CS201-001
- Bob Jones enrolled in CS101-001
- All enrollments with status: `Enrolled`
- No grades assigned yet (pending completion)

### Seeding Logic

1. **Idempotency**: Checks if data exists before seeding
   ```csharp
   if (await _context.Users.AnyAsync())
   {
       _logger.LogInformation("Tenant database already contains data. Skipping seed.");
       return;
   }
   ```

2. **Dependency Order**: Seeds data in correct order to satisfy foreign key constraints
   - Users (Faculty, Students, Staff)
   - Programs
   - Courses
   - Sections
   - Enrollments

3. **Transaction Safety**: All seeding happens within SaveChangesAsync() calls

## Application Integration

### Program.cs Setup

```csharp
using UMS.Infrastructure.Extensions;

var app = builder.Build();

// ... middleware configuration ...

// Initialize and seed databases
await app.InitializeDatabasesAsync();

app.Run();
```

### On-Demand Tenant Seeding

When a new tenant database is created via `DatabaseInitializationService.EnsureDatabaseCreatedAsync()`:

1. Apply all pending migrations
2. Check if database is empty (`!context.Users.AnyAsync()`)
3. If empty, invoke `TenantDbSeeder.SeedAsync()`

## Testing and Verification

### Manual Verification Steps

1. **Delete Existing Databases** (if testing from scratch):
   ```powershell
   # SQL Server LocalDB
   sqllocaldb stop mssqllocaldb
   sqllocaldb delete mssqllocaldb
   sqllocaldb create mssqllocaldb
   sqllocaldb start mssqllocaldb
   ```

2. **Run Application**:
   ```bash
   dotnet run --project UMS.api
   ```

3. **Check Master Database**:
   - Verify `Tenants` table contains "Demo University" with subdomain "demo"

4. **Trigger Tenant Database Creation**:
   - Make any API request to `https://demo.localhost:5001/api/...`
   - OR use `TenantsController.Post()` to create tenant explicitly

5. **Verify Tenant Database**:
   - Check database `UMS_Tenant_demo` exists
   - Verify tables: Users, Students, Faculty, Staff, Programs, Courses, Sections, Enrollments
   - Verify sample data counts:
     - 3 Users (1 admin, 1 faculty, 2 students)
     - 2 Programs
     - 3 Courses
     - 2 Sections
     - 3 Enrollments

### Automated Testing

Current test suite (16 passing tests) covers:
- Middleware functionality
- Error handling
- Tenant resolution

**Future test additions**:
- Database seeding unit tests
- Integration tests for seeded data
- Seed data validation tests

## Security Considerations

### Password Hashing

⚠️ **IMPORTANT**: The current seed data uses placeholder BCrypt hashes.

**Production Recommendations**:
1. Generate unique strong passwords for each environment
2. Use proper password hashing (BCrypt, Argon2)
3. Store initial admin credentials securely
4. Force password change on first login
5. Consider using email-based activation instead of default passwords

### Connection Strings

⚠️ **IMPORTANT**: Connection strings are currently generated in code.

**Production Recommendations**:
1. Store connection strings in Azure Key Vault, AWS Secrets Manager, or similar
2. Use managed identities for database authentication (Azure SQL, AWS RDS)
3. Encrypt sensitive configuration data
4. Implement connection string rotation policies
5. Never commit production credentials to source control

## Configuration

### Database Provider Selection

Set in `appsettings.json`:

```json
{
  "DatabaseProvider": "SqlServer",  // or "PostgreSQL"
  "ConnectionStrings": {
    "MasterConnection": "Server=(localdb)\\mssqllocaldb;Database=UMS_Master;..."
  }
}
```

### Customizing Seed Data

To customize seed data:

1. Edit `MasterDbSeeder.cs` for tenant defaults
2. Edit `TenantDbSeeder.cs` for academic data templates
3. Consider creating environment-specific seeders (Development, Staging, Production)

### Disabling Seeding

To skip seeding (e.g., in production after initial setup):

```csharp
// In Program.cs, comment out or conditionally call:
if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabasesAsync();
}
```

## Troubleshooting

### Seed Data Not Appearing

**Symptoms**: Database is empty after startup

**Potential Causes**:
1. Migrations haven't been applied
2. Seeding was skipped due to existing data check
3. Exception during seeding (check logs)

**Solutions**:
```bash
# Apply migrations manually
dotnet ef database update --project UMS.Infrastructure --startup-project UMS.api

# Check logs for seeding errors
# Set log level to Debug in appsettings.json
"Logging": {
  "LogLevel": {
    "Default": "Debug"
  }
}
```

### Connection String Errors

**Symptoms**: Cannot create tenant database

**Solutions**:
1. Verify database provider setting matches actual database
2. Check LocalDB service is running (SQL Server)
3. Verify PostgreSQL service is accessible
4. Review connection string format for the provider

### Seed Data Conflicts

**Symptoms**: Unique constraint violations during seeding

**Solutions**:
1. Drop and recreate databases
2. Ensure seeding is idempotent (checks for existing data)
3. Review entity unique constraints (email, student number, etc.)

## Migration Strategy

### Updating Seed Data

When modifying seed data:

1. **Don't modify existing migrations**
2. **Option A**: Create a data migration
   ```bash
   dotnet ef migrations add UpdateSeedData --project UMS.Infrastructure --startup-project UMS.api
   ```

3. **Option B**: Update seeder classes (affects new databases only)
   - Existing databases retain old seed data
   - Consider creating a separate data update migration

### Production Considerations

- **First Deployment**: Seeding runs automatically
- **Subsequent Deployments**: Only migrations run, seeding is skipped
- **New Tenants**: Each tenant database is seeded when created
- **Data Updates**: Use migrations for schema changes, separate scripts for data updates

## Best Practices

1. **Keep Seed Data Minimal**: Only essential data for application to function
2. **Environment-Specific Seeding**: Different data for Dev/Staging/Production
3. **Version Control**: Seed data should be versioned like migrations
4. **Documentation**: Document default credentials and test accounts
5. **Security**: Never use weak passwords in production
6. **Logging**: Log seeding operations for audit trail
7. **Idempotency**: Always check if data exists before inserting
8. **Dependencies**: Respect foreign key relationships in seeding order

## Future Enhancements

1. **Environment-Based Seeders**: Different seeders for Development, Testing, Production
2. **Seed Data Configuration**: Load seed data from JSON/YAML files
3. **Localization**: Support multiple languages in seed data
4. **Bulk Seeding**: Add command-line tools for bulk tenant creation
5. **Seed Profiles**: Different seed configurations (minimal, full, demo)
6. **Validation**: Add seed data validation before insertion
7. **Rollback**: Support for removing seed data
8. **Import/Export**: Tools for exporting/importing seed data configurations
