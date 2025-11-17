# Simple SuperAdmin Tenant Creation Test
Write-Host "=== Simple Tenant Creation Test ===" -ForegroundColor Cyan

$baseUrl = "http://localhost:5263"

# Login
Write-Host "Logging in..." -ForegroundColor Yellow
$loginBody = @{
    email = "superadmin@umsoperaedu.com"
    password = "SuperAdmin@123"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/superadmin/login" `
    -Method Post `
    -Headers @{"Host" = "admin.localhost"} `
    -Body $loginBody `
    -ContentType "application/json"

$token = $loginResponse.token
Write-Host "[SUCCESS] Logged in" -ForegroundColor Green

# Create simple tenant
Write-Host "`nCreating tenant..." -ForegroundColor Yellow
$randomNum = Get-Random -Minimum 10000 -Maximum 99999
$newTenant = @{
    name = "Harvard University"
    subdomain = "harvard$randomNum"
    adminEmail = "admin@harvard.edu"
} | ConvertTo-Json

try {
    $createResponse = Invoke-RestMethod -Uri "$baseUrl/api/tenants" `
        -Method Post `
        -Headers @{
            "Authorization" = "Bearer $token"
            "Host" = "admin.localhost"
        } `
        -Body $newTenant `
        -ContentType "application/json"

    Write-Host "[SUCCESS] Tenant created!" -ForegroundColor Green
    Write-Host "Tenant ID: $($createResponse.id)" -ForegroundColor Gray
    Write-Host "Subdomain: $($createResponse.subdomain)" -ForegroundColor Gray
} catch {
    Write-Host "[FAILED] Error: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorStream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorStream)
        Write-Host $reader.ReadToEnd() -ForegroundColor Red
    }
}
