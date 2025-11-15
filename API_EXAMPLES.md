# API Testing Examples

## Base URL
```
http://localhost:5263
```

## 1. Create a Student

### Request
```http
POST /api/students
Content-Type: application/json

{
  "email": "jane.smith@university.edu",
  "password": "Password123!",
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "2002-05-15",
  "studentNumber": "S2024001",
  "enrollmentDate": "2024-09-01",
  "programId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Success Response (201 Created)
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000"
}
```

### Error Response (400 Bad Request - Validation)
```json
{
  "statusCode": 400,
  "title": "Validation failed",
  "detail": "One or more validation errors occurred.",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": {
    "Email": ["Email is required.", "Email must be a valid email address."],
    "Password": ["Password must be at least 8 characters."],
    "ProgramId": ["ProgramId is required."]
  }
}
```

## 2. Get All Students

### Request
```http
GET /api/students
```

### Success Response (200 OK)
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "jane.smith@university.edu",
    "firstName": "Jane",
    "lastName": "Smith",
    "phoneNumber": "+1234567890",
    "dateOfBirth": "2002-05-15",
    "studentNumber": "S2024001",
    "enrollmentDate": "2024-09-01",
    "expectedGraduationDate": null,
    "programId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "academicStatus": 0,
    "gpa": 0.0,
    "totalCredits": 0,
    "isActive": true
  }
]
```

## 3. Get Student by ID

### Request
```http
GET /api/students/550e8400-e29b-41d4-a716-446655440000
```

### Success Response (200 OK)
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "jane.smith@university.edu",
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "2002-05-15",
  "studentNumber": "S2024001",
  "enrollmentDate": "2024-09-01",
  "expectedGraduationDate": null,
  "programId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "academicStatus": 0,
  "gpa": 0.0,
  "totalCredits": 0,
  "isActive": true
}
```

### Error Response (404 Not Found)
```json
{
  "statusCode": 404,
  "title": "Resource not found",
  "detail": "Student with ID 550e8400-e29b-41d4-a716-446655440000 not found",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

## 4. Create a Course

### Request
```http
POST /api/courses
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "code": "CS101",
  "name": "Introduction to Computer Science",
  "description": "Fundamental concepts of computer science",
  "credits": 3,
  "departmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Success Response (201 Created)
```json
{
  "id": "7d8e9f00-1234-5678-9abc-def012345678"
}
```

### Error Response (401 Unauthorized)
```json
{
  "statusCode": 401,
  "title": "Unauthorized",
  "detail": "Unauthorized access",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

### Error Response (403 Forbidden)
```json
{
  "statusCode": 403,
  "title": "Forbidden",
  "detail": "User does not have the required role: FacultyOrAdmin",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

## 5. Create Enrollment

### Request
```http
POST /api/enrollments
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "studentId": "550e8400-e29b-41d4-a716-446655440000",
  "sectionId": "7d8e9f00-1234-5678-9abc-def012345678",
  "enrollmentDate": "2025-01-15"
}
```

### Success Response (201 Created)
```json
{
  "id": "9a8b7c6d-5e4f-3a2b-1c0d-fedcba098765"
}
```

### Error Response (400 Bad Request - Business Rule)
```json
{
  "statusCode": 400,
  "title": "Bad request",
  "detail": "Student has already enrolled in this section",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

## 6. Update Enrollment Grade

### Request
```http
PUT /api/enrollments/9a8b7c6d-5e4f-3a2b-1c0d-fedcba098765/grade
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "numericGrade": 85.5,
  "gradePoints": 3.5
}
```

### Success Response (204 No Content)
```
(empty body)
```

### Error Response (404 Not Found)
```json
{
  "statusCode": 404,
  "title": "Resource not found",
  "detail": "Enrollment with ID 9a8b7c6d-5e4f-3a2b-1c0d-fedcba098765 not found",
  "traceId": "00-1234567890abcdef-1234567890abcdef-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": null
}
```

## Testing with cURL

### Create Student
```bash
curl -X POST "http://localhost:5263/api/students" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@university.edu",
    "password": "Password123!",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "dateOfBirth": "2001-03-20",
    "studentNumber": "S2024002",
    "enrollmentDate": "2024-09-01",
    "programId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }'
```

### Get All Students
```bash
curl -X GET "http://localhost:5263/api/students"
```

### Get Student by ID
```bash
curl -X GET "http://localhost:5263/api/students/550e8400-e29b-41d4-a716-446655440000"
```

## Testing with PowerShell

### Create Student
```powershell
$body = @{
    email = "alice.johnson@university.edu"
    password = "Password123!"
    firstName = "Alice"
    lastName = "Johnson"
    phoneNumber = "+1234567890"
    dateOfBirth = "2001-07-10"
    studentNumber = "S2024003"
    enrollmentDate = "2024-09-01"
    programId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5263/api/students" `
  -Method Post `
  -Body $body `
  -ContentType "application/json"
```

### Get All Students
```powershell
Invoke-RestMethod -Uri "http://localhost:5263/api/students" -Method Get
```

### Get Student by ID
```powershell
$studentId = "550e8400-e29b-41d4-a716-446655440000"
Invoke-RestMethod -Uri "http://localhost:5263/api/students/$studentId" -Method Get
```

## Field Validations

### Email
- Required
- Must be valid email format

### Password
- Required
- Minimum 6 characters

### FirstName
- Required
- Maximum 50 characters

### LastName
- Required
- Maximum 50 characters

### DateOfBirth
- Required
- Must be in the past

### EnrollmentDate
- Required

### ProgramId
- Required
- Must be valid GUID

## Academic Status Enum Values

```
0 = Active
1 = Probation
2 = Suspended
3 = Graduated
4 = Withdrawn
```

## User Type Enum Values

```
0 = Student
1 = Faculty
2 = Staff
3 = Administrator
```

## Standardized Error Response Format

All error responses follow the `ErrorResponse` model defined in `UMS.api/Models/ErrorResponse.cs`:

```json
{
  "statusCode": 400,
  "title": "Error title",
  "detail": "Detailed error message",
  "traceId": "00-trace-id-00",
  "timestampUtc": "2025-11-14T10:30:00Z",
  "errors": {
    "PropertyName": ["Error message 1", "Error message 2"]
  }
}
```

### Error Response Fields

| Field | Type | Description |
|-------|------|-------------|
| `statusCode` | integer | HTTP status code (400, 401, 404, 500, etc.) |
| `title` | string | Brief error summary |
| `detail` | string | Detailed error description |
| `traceId` | string | Request trace identifier for debugging |
| `timestampUtc` | datetime | When the error occurred (UTC) |
| `errors` | object | Validation errors grouped by property name (null for non-validation errors) |

## Common HTTP Status Codes

| Code | Title | When Used |
|------|-------|-----------|
| 400  | Bad Request / Validation failed | Invalid input, validation errors, business rule violations |
| 401  | Unauthorized | Missing or invalid authentication token |
| 403  | Forbidden | User lacks required permissions/role |
| 404  | Resource not found | Requested entity does not exist |
| 500  | Internal server error | Unexpected server-side error |

## Notes

- All dates should be in ISO 8601 format: `YYYY-MM-DD` or `YYYY-MM-DDTHH:mm:ss`
- All IDs are GUIDs (UUIDs)
- Passwords are automatically hashed using BCrypt
- The API uses soft deletes (IsDeleted flag), so deleted records aren't physically removed
