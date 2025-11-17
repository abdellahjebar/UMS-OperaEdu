# Detailed SuperAdmin Tenant Creation Test
# Tests creating a tenant with detailed error reporting

Write-Host "=== SuperAdmin Tenant Creation Detailed Test ===" -ForegroundColor Cyan
Write-Host ""

# Configuration
$baseUrl = "http://localhost:5263"
$superAdminEmail = "superadmin@umsoperaedu.com"
$superAdminPassword = "SuperAdmin@123"

# Step 1: Login
Write-Host "Step 1: Logging in as SuperAdmin..." -ForegroundColor Yellow

$loginBody = @{
    email = $superAdminEmail
    password = $superAdminPassword
} | ConvertTo-Json

try {
    $headers = @{
        "Host" = "admin.localhost"
    }
    
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/superadmin/login" `
        -Method Post `
        -Headers $headers `
        -Body $loginBody `
        -ContentType "application/json" `
        -ErrorAction Stop

    Write-Host "[SUCCESS] Login successful!" -ForegroundColor Green
    $token = $loginResponse.token
    Write-Host ""

} catch {
    Write-Host "[FAILED] Login failed: $_" -ForegroundColor Red
    exit 1
}

# Step 2: Try to create tenant with detailed error handling
Write-Host "Step 2: Creating new tenant..." -ForegroundColor Yellow

$randomNum = Get-Random -Minimum 10000 -Maximum 99999
$newTenant = @{
    name = "Test University $randomNum"
    subdomain = "testuni$randomNum"
    adminEmail = "admin@testuni$randomNum.edu"
    adminPhone = "+1-555-0199"
    address = "123 Test Street, Test City"
    subscriptionStartDate = (Get-Date).ToString("yyyy-MM-dd")
    subscriptionEndDate = (Get-Date).AddYears(1).ToString("yyyy-MM-dd")
    annualFee = 10000
    preferredGradingSystem = "Numeric"
    maxStudents = 1000
    maxFaculty = 100
    maxCourses = 500
} | ConvertTo-Json

Write-Host "Request Body:" -ForegroundColor Gray
Write-Host $newTenant -ForegroundColor Gray
Write-Host ""

try {
    $headers = @{
        "Authorization" = "Bearer $token"
        "Host" = "admin.localhost"
    }

    $createResponse = Invoke-WebRequest -Uri "$baseUrl/api/tenants" `
        -Method Post `
        -Headers $headers `
        -Body $newTenant `
        -ContentType "application/json" `
        -ErrorAction Stop

    Write-Host "[SUCCESS] Tenant created successfully!" -ForegroundColor Green
    Write-Host "Status Code: $($createResponse.StatusCode)" -ForegroundColor Gray
    Write-Host "Response:" -ForegroundColor Gray
    Write-Host $createResponse.Content -ForegroundColor Gray

} catch {
    Write-Host "[FAILED] Failed to create tenant!" -ForegroundColor Red
    Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    
    # Try to get detailed error message
    try {
        $errorStream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorStream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "Error Details:" -ForegroundColor Red
        Write-Host $errorBody -ForegroundColor Red
    } catch {
        Write-Host "Could not read error details" -ForegroundColor Red
    }
}
