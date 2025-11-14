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

### Error Response (400 Bad Request)
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "property": "Email",
      "message": "Email is required."
    },
    {
      "property": "Password",
      "message": "Password must be at least 6 characters."
    }
  ]
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
  "message": "Student with ID '550e8400-e29b-41d4-a716-446655440000' not found."
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

## Common Error Codes

| Code | Description |
|------|-------------|
| 400  | Bad Request - Validation failed or invalid data |
| 404  | Not Found - Resource doesn't exist |
| 401  | Unauthorized - Not authenticated |
| 500  | Internal Server Error - Something went wrong |

## Notes

- All dates should be in ISO 8601 format: `YYYY-MM-DD` or `YYYY-MM-DDTHH:mm:ss`
- All IDs are GUIDs (UUIDs)
- Passwords are automatically hashed using BCrypt
- The API uses soft deletes (IsDeleted flag), so deleted records aren't physically removed
