# SuperAdmin Authentication Test Script
# Tests the new JWT-based SuperAdmin authentication system
# Run from: scripts/test-superadmin.ps1

Write-Host "=== SuperAdmin Authentication Test ===" -ForegroundColor Cyan
Write-Host ""

# Configuration
$baseUrl = "http://localhost:5263"
$superAdminEmail = "superadmin@umsoperaedu.com"
$superAdminPassword = "SuperAdmin@123"

Write-Host "Step 1: Testing SuperAdmin Login" -ForegroundColor Yellow
Write-Host "Endpoint: POST $baseUrl/api/superadmin/login"
Write-Host "Note: Using localhost - in production use admin.yourdomain.com" -ForegroundColor Gray
Write-Host ""

$loginBody = @{
    email = $superAdminEmail
    password = $superAdminPassword
} | ConvertTo-Json

try {
    # Use Host header to simulate admin.localhost subdomain
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
    Write-Host "  Email: $($loginResponse.email)" -ForegroundColor Gray
    Write-Host "  Name: $($loginResponse.fullName)" -ForegroundColor Gray
    Write-Host "  Message: $($loginResponse.message)" -ForegroundColor Gray
    Write-Host ""

    $token = $loginResponse.token
    Write-Host "JWT Token (first 50 chars): $($token.Substring(0, [Math]::Min(50, $token.Length)))..." -ForegroundColor Gray
    Write-Host ""

} catch {
    Write-Host "[FAILED] Login failed!" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}

# Step 2: Test Get All Tenants (with JWT)
Write-Host "Step 2: Testing Get All Tenants (Authenticated)" -ForegroundColor Yellow
Write-Host "Endpoint: GET $baseUrl/api/tenants"

try {
    $headers = @{
        "Authorization" = "Bearer $token"
        "Host" = "admin.localhost"
    }

    $tenants = Invoke-RestMethod -Uri "$baseUrl/api/tenants" `
        -Method Get `
        -Headers $headers `
        -ErrorAction Stop

    Write-Host "[SUCCESS] Successfully retrieved tenants!" -ForegroundColor Green
    Write-Host "  Total tenants: $($tenants.Count)" -ForegroundColor Gray
    
    foreach ($tenant in $tenants) {
        Write-Host "  - $($tenant.name) (subdomain: $($tenant.subdomain))" -ForegroundColor Gray
    }
    Write-Host ""

} catch {
    Write-Host "[FAILED] Failed to retrieve tenants!" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}

# Step 3: Test Get Tenants without JWT (should fail)
Write-Host "Step 3: Testing Get All Tenants (Unauthenticated - Should Fail)" -ForegroundColor Yellow
Write-Host "Endpoint: GET $baseUrl/api/tenants"

try {
    $headers = @{
        "Host" = "admin.localhost"
    }
    
    $unauthorizedAttempt = Invoke-RestMethod -Uri "$baseUrl/api/tenants" `
        -Method Get `
        -Headers $headers `
        -ErrorAction Stop

    Write-Host "[FAILED] SECURITY ISSUE: Unauthenticated request succeeded!" -ForegroundColor Red
    exit 1

} catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "[SUCCESS] Correctly rejected unauthenticated request (401 Unauthorized)" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host "[FAILED] Unexpected error: $_" -ForegroundColor Red
        exit 1
    }
}

# Step 4: Test Create Tenant with JWT
Write-Host "Step 4: Testing Create Tenant (Authenticated)" -ForegroundColor Yellow
Write-Host "Endpoint: POST $baseUrl/api/tenants"

$newTenant = @{
    name = "Test University $(Get-Random -Minimum 1000 -Maximum 9999)"
    subdomain = "testuni$(Get-Random -Minimum 1000 -Maximum 9999)"
    adminEmail = "admin@testuni.edu"
    adminPhone = "+1-555-0199"
    address = "123 Test Street, Test City"
} | ConvertTo-Json

try {
    $headers = @{
        "Authorization" = "Bearer $token"
        "Host" = "admin.localhost"
    }

    $createResponse = Invoke-RestMethod -Uri "$baseUrl/api/tenants" `
        -Method Post `
        -Headers $headers `
        -Body $newTenant `
        -ContentType "application/json" `
        -ErrorAction Stop

    Write-Host "[SUCCESS] Tenant created successfully!" -ForegroundColor Green
    Write-Host "  Tenant ID: $($createResponse.id)" -ForegroundColor Gray
    Write-Host "  Subdomain: $($createResponse.subdomain)" -ForegroundColor Gray
    Write-Host ""

} catch {
    Write-Host "[FAILED] Failed to create tenant!" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
}

Write-Host "=== Test Summary ===" -ForegroundColor Cyan
Write-Host "[OK] SuperAdmin JWT authentication working correctly" -ForegroundColor Green
Write-Host "[OK] Authorization policies enforced" -ForegroundColor Green
Write-Host "[OK] Tenant management endpoints secured" -ForegroundColor Green
Write-Host ""
Write-Host "Default SuperAdmin Credentials:" -ForegroundColor Yellow
Write-Host "  Email: $superAdminEmail" -ForegroundColor Gray
Write-Host "  Password: $superAdminPassword" -ForegroundColor Gray
Write-Host "  [WARNING] Please change the default password in production!" -ForegroundColor Red
