# Comprehensive API Testing Script for UMS
# Tests all controllers with proper tenant context

$ErrorActionPreference = "Continue"
$baseUrl = "http://127.0.0.1:5263"
$tenant = "testuniversity"
$tenantHost = "$tenant.localhost:5263"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "UMS API Comprehensive Test Suite" -ForegroundColor Cyan
Write-Host "Testing Tenant: $tenant" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Test Results Tracking
$testResults = @()

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Uri,
        [hashtable]$Headers = @{ Host = $tenantHost },
        [object]$Body = $null,
        [int]$ExpectedStatus = 200
    )
    
    Write-Host "`n--- Testing: $Name ---" -ForegroundColor Yellow
    Write-Host "Method: $Method | URI: $Uri" -ForegroundColor Gray
    
    try {
        $params = @{
            Uri = "$baseUrl$Uri"
            Method = $Method
            Headers = $Headers
            ContentType = "application/json"
        }
        
        if ($Body) {
            $params['Body'] = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-RestMethod @params -ErrorAction Stop
        
        Write-Host "[SUCCESS]" -ForegroundColor Green
        Write-Host "Response:" -ForegroundColor Gray
        Write-Host ($response | ConvertTo-Json -Depth 3)
        
        $script:testResults += [PSCustomObject]@{
            Endpoint = $Name
            Status = "PASS"
            Message = "Status $ExpectedStatus"
        }
        
        return $response
        
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq $ExpectedStatus) {
            Write-Host "[EXPECTED] FAILURE ($statusCode)" -ForegroundColor Green
            $script:testResults += [PSCustomObject]@{
                Endpoint = $Name
                Status = "PASS"
                Message = "Expected status $ExpectedStatus"
            }
        }
        else {
            Write-Host "[FAILED]" -ForegroundColor Red
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
            $script:testResults += [PSCustomObject]@{
                Endpoint = $Name
                Status = "FAIL"
                Message = $_.Exception.Message
            }
        }
        return $null
    }
}

# ==========================================
# 1. AUTHENTICATION
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "1. AUTHENTICATION TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Login with admin
$loginBody = @{
    email = "admin@testuniversity.edu"
    password = "Student@123"
}
$loginResponse = Test-Endpoint -Name "Auth: Login (Admin)" -Method "POST" -Uri "/api/auth/login" -Body $loginBody

if ($loginResponse -and $loginResponse.accessToken) {
    $adminToken = $loginResponse.accessToken
    Write-Host "`nAdmin Token captured (first 50 chars): $($adminToken.Substring(0, [Math]::Min(50, $adminToken.Length)))..." -ForegroundColor Green
    
    $authHeaders = @{
        Host = $tenantHost
        Authorization = "Bearer $adminToken"
    }
} else {
    Write-Host "`n⚠ Admin login failed, subsequent tests will fail" -ForegroundColor Red
    $authHeaders = @{ Host = $tenantHost }
}

# Test invalid login
$invalidLogin = @{
    email = "invalid@test.com"
    password = "wrong"
}
Test-Endpoint -Name "Auth: Login (Invalid)" -Method "POST" -Uri "/api/auth/login" -Body $invalidLogin -ExpectedStatus 401

# ==========================================
# 2. STUDENTS
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "2. STUDENTS TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get all students
$studentsResponse = Test-Endpoint -Name "Students: Get All" -Method "GET" -Uri "/api/students" -Headers $authHeaders

# Get student by ID (if any exist)
if ($studentsResponse -and $studentsResponse.Count -gt 0) {
    $studentId = $studentsResponse[0].id
    Test-Endpoint -Name "Students: Get By ID" -Method "GET" -Uri "/api/students/$studentId" -Headers $authHeaders
}

# Create a temporary department and program for student creation test
$tempDept = @{
    code = "TEMP$(Get-Random -Maximum 999)"
    name = "Temporary Department"
    description = "Created for student testing"
}
$tempDeptCreated = Invoke-RestMethod -Uri "http://127.0.0.1:5263/api/departments" -Method Post -Body ($tempDept | ConvertTo-Json) -ContentType "application/json" -Headers $authHeaders -ErrorAction SilentlyContinue
$validProgramId = $null
if ($tempDeptCreated -and $tempDeptCreated.id) {
    $tempProgram = @{
        code = "TMPPROG$(Get-Random -Maximum 999)"
        name = "Temporary Program"
        description = "Created for student testing"
        departmentId = $tempDeptCreated.id
        degreeType = 3  # Bachelor
        durationYears = 4
        requiredCredits = 120
    }
    $tempProgramCreated = Invoke-RestMethod -Uri "http://127.0.0.1:5263/api/programs" -Method Post -Body ($tempProgram | ConvertTo-Json) -ContentType "application/json" -Headers $authHeaders -ErrorAction SilentlyContinue
    if ($tempProgramCreated -and $tempProgramCreated.id) {
        $validProgramId = $tempProgramCreated.id
    }
}

# Create new student with valid ProgramId
$newStudent = @{
    firstName = "Test"
    lastName = "Student"
    email = "test.student.$(Get-Random)@testuniversity.edu"
    password = "Test@123"
    dateOfBirth = "2000-01-01T00:00:00Z"
    phoneNumber = "555-0100"
    enrollmentDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
    programId = if ($validProgramId) { $validProgramId } else { "00000000-0000-0000-0000-000000000000" }
}
$createdStudent = Test-Endpoint -Name "Students: Create" -Method "POST" -Uri "/api/students" -Headers $authHeaders -Body $newStudent

# Update student
if ($createdStudent -and $createdStudent.id) {
    $updateStudent = @{
        id = $createdStudent.id
        firstName = $newStudent.firstName
        lastName = $newStudent.lastName
        phoneNumber = "555-0199"
        dateOfBirth = $newStudent.dateOfBirth
        programId = $newStudent.programId
        academicStatus = 1
    }
    Test-Endpoint -Name "Students: Update" -Method "PUT" -Uri "/api/students/$($createdStudent.id)" -Headers $authHeaders -Body $updateStudent
    
    # Delete student
    Test-Endpoint -Name "Students: Delete" -Method "DELETE" -Uri "/api/students/$($createdStudent.id)" -Headers $authHeaders -ExpectedStatus 204
}

# ==========================================
# 3. DEPARTMENTS
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "3. DEPARTMENTS TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get all departments
$departmentsResponse = Test-Endpoint -Name "Departments: Get All" -Method "GET" -Uri "/api/departments" -Headers $authHeaders

# Get department by ID
if ($departmentsResponse -and $departmentsResponse.Count -gt 0) {
    $deptId = $departmentsResponse[0].id
    Test-Endpoint -Name "Departments: Get By ID" -Method "GET" -Uri "/api/departments/$deptId" -Headers $authHeaders
}

# Create department
$newDept = @{
    code = "TST$(Get-Random -Maximum 999)"
    name = "Test Department"
    description = "Test department for API testing"
}
$createdDept = Test-Endpoint -Name "Departments: Create" -Method "POST" -Uri "/api/departments" -Headers $authHeaders -Body $newDept

# Update department
if ($createdDept -and $createdDept.id) {
    $updateDept = @{
        id = $createdDept.id
        code = "TST$(Get-Random -Maximum 999)"
        name = "Updated Test Department"
        description = "Updated description"
    }
    Test-Endpoint -Name "Departments: Update" -Method "PUT" -Uri "/api/departments/$($createdDept.id)" -Headers $authHeaders -Body $updateDept
    
    # Delete department
    Test-Endpoint -Name "Departments: Delete" -Method "DELETE" -Uri "/api/departments/$($createdDept.id)" -Headers $authHeaders -ExpectedStatus 204
}

# ==========================================
# 4. PROGRAMS
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "4. PROGRAMS TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get all programs
$programsResponse = Test-Endpoint -Name "Programs: Get All" -Method "GET" -Uri "/api/programs" -Headers $authHeaders

# Get program by ID
if ($programsResponse -and $programsResponse.Count -gt 0) {
    $programId = $programsResponse[0].id
    Test-Endpoint -Name "Programs: Get By ID" -Method "GET" -Uri "/api/programs/$programId" -Headers $authHeaders
}

# ==========================================
# 5. COURSES
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "5. COURSES TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get all courses
$coursesResponse = Test-Endpoint -Name "Courses: Get All" -Method "GET" -Uri "/api/courses" -Headers $authHeaders

# Get course by ID
if ($coursesResponse -and $coursesResponse.Count -gt 0) {
    $courseId = $coursesResponse[0].id
    Test-Endpoint -Name "Courses: Get By ID" -Method "GET" -Uri "/api/courses/$courseId" -Headers $authHeaders
}

# Create course (requires department)
if ($departmentsResponse -and $departmentsResponse.Count -gt 0) {
    $deptId = $departmentsResponse[0].id
    $newCourse = @{
        code = "TST$(Get-Random -Maximum 999)"
        name = "Test Course"
        description = "Test course for API testing"
        credits = 3
        departmentId = $deptId
    }
    $createdCourse = Test-Endpoint -Name "Courses: Create" -Method "POST" -Uri "/api/courses" -Headers $authHeaders -Body $newCourse
    
    # Update course
    if ($createdCourse -and $createdCourse.id) {
        $updateCourse = @{
            id = $createdCourse.id
            code = $newCourse.code
            name = "Updated Course Name"
            description = "Updated description"
            credits = 4
            departmentId = $newCourse.departmentId
        }
        Test-Endpoint -Name "Courses: Update" -Method "PUT" -Uri "/api/courses/$($createdCourse.id)" -Headers $authHeaders -Body $updateCourse
        
        # Delete course
        Test-Endpoint -Name "Courses: Delete" -Method "DELETE" -Uri "/api/courses/$($createdCourse.id)" -Headers $authHeaders -ExpectedStatus 204
    }
}

# ==========================================
# 6. FACULTY
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "6. FACULTY TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get all faculty
$facultyResponse = Test-Endpoint -Name "Faculty: Get All" -Method "GET" -Uri "/api/faculty" -Headers $authHeaders

# Get faculty by ID
if ($facultyResponse -and $facultyResponse.Count -gt 0) {
    $facultyId = $facultyResponse[0].id
    Test-Endpoint -Name "Faculty: Get By ID" -Method "GET" -Uri "/api/faculty/$facultyId" -Headers $authHeaders
}

# ==========================================
# 7. SECTIONS
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "7. SECTIONS TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get all sections
$sectionsResponse = Test-Endpoint -Name "Sections: Get All" -Method "GET" -Uri "/api/sections" -Headers $authHeaders

# Get section by ID
if ($sectionsResponse -and $sectionsResponse.Count -gt 0) {
    $sectionId = $sectionsResponse[0].id
    Test-Endpoint -Name "Sections: Get By ID" -Method "GET" -Uri "/api/sections/$sectionId" -Headers $authHeaders
}

# ==========================================
# 8. ENROLLMENTS
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "8. ENROLLMENTS TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get all enrollments
$enrollmentsResponse = Test-Endpoint -Name "Enrollments: Get All" -Method "GET" -Uri "/api/enrollments" -Headers $authHeaders

# Get enrollment by ID
if ($enrollmentsResponse -and $enrollmentsResponse.Count -gt 0) {
    $enrollmentId = $enrollmentsResponse[0].id
    Test-Endpoint -Name "Enrollments: Get By ID" -Method "GET" -Uri "/api/enrollments/$enrollmentId" -Headers $authHeaders
}

# Get enrollments by student
if ($studentsResponse -and $studentsResponse.Count -gt 0) {
    $studentId = $studentsResponse[0].id
    Test-Endpoint -Name "Enrollments: Get By Student" -Method "GET" -Uri "/api/enrollments/student/$studentId" -Headers $authHeaders
}

# ==========================================
# 9. TENANTS (Super Admin Only - Expected to fail with tenant context)
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "9. TENANTS TESTS (Expected to fail - requires super admin)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Test-Endpoint -Name "Tenants: Get All" -Method "GET" -Uri "/api/tenants" -Headers $authHeaders -ExpectedStatus 404

# ==========================================
# TEST SUMMARY
# ==========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "TEST SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$passCount = ($testResults | Where-Object { $_.Status -eq "PASS" }).Count
$failCount = ($testResults | Where-Object { $_.Status -eq "FAIL" }).Count
$totalCount = $testResults.Count

Write-Host "`nTotal Tests: $totalCount" -ForegroundColor White
Write-Host "Passed: $passCount" -ForegroundColor Green
Write-Host "Failed: $failCount" -ForegroundColor $(if ($failCount -gt 0) { "Red" } else { "Green" })

Write-Host "`nDetailed Results:" -ForegroundColor White
$testResults | Format-Table -AutoSize

if ($failCount -eq 0) {
    Write-Host "`n[PASS] All tests passed!" -ForegroundColor Green
}
else {
    Write-Host "`n[FAIL] Some tests failed. Review details above." -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Testing Complete" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan


