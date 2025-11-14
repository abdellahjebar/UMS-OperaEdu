# Local Testing Script
# Test your changes locally before committing

param(
    [switch]$SkipBuild,
    [switch]$SkipApi
)

# Change to solution directory
Set-Location $PSScriptRoot\..

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  LOCAL TESTING WORKFLOW" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorCount = 0

# Step 1: Clean and Restore
if (-not $SkipBuild) {
    Write-Host "[1/4] Cleaning and restoring..." -ForegroundColor Yellow
    dotnet clean --nologo --verbosity quiet
    dotnet restore --nologo
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Restore failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Restore successful" -ForegroundColor Green
    Write-Host ""
}

# Step 2: Build
if (-not $SkipBuild) {
    Write-Host "[2/4] Building solution..." -ForegroundColor Yellow
    dotnet build --configuration Debug --no-restore
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Build failed - Fix errors before testing" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Build successful" -ForegroundColor Green
    Write-Host ""
}

# Step 3: Check for running API
Write-Host "[3/4] Checking for running API..." -ForegroundColor Yellow
$apiProcess = Get-Process -Name "UMS.API" -ErrorAction SilentlyContinue
if ($apiProcess) {
    Write-Host "⚠ API is already running (PID: $($apiProcess.Id))" -ForegroundColor Yellow
    Write-Host "  Stop it with: Stop-Process -Id $($apiProcess.Id)" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "✓ No running API instance found" -ForegroundColor Green
    Write-Host ""
}

# Step 4: Start API for testing
if (-not $SkipApi) {
    Write-Host "[4/4] Starting API for testing..." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "API will start on: http://localhost:5263" -ForegroundColor Green
    Write-Host "Swagger UI: http://localhost:5263/swagger" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Press CTRL+C to stop the API when done testing" -ForegroundColor Yellow
    Write-Host ""
    
    # Start the API
    Set-Location ".\UMS.api"
    dotnet run
} else {
    Write-Host "[4/4] Skipping API startup" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "BUILD COMPLETE - Ready for manual testing" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "To start the API manually:" -ForegroundColor Cyan
    Write-Host "  cd UMS.api" -ForegroundColor White
    Write-Host "  dotnet run" -ForegroundColor White
}
