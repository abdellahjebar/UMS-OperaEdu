# Pre-Push Validation Script
# Run this before pushing to production to catch issues early

# Change to solution directory
Set-Location $PSScriptRoot\..

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  PRE-PUSH VALIDATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorCount = 0

# Step 1: Clean previous builds
Write-Host "[1/5] Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Clean failed" -ForegroundColor Red
    $ErrorCount++
} else {
    Write-Host "✓ Clean successful" -ForegroundColor Green
}
Write-Host ""

# Step 2: Restore dependencies
Write-Host "[2/5] Restoring dependencies..." -ForegroundColor Yellow
dotnet restore --nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Restore failed" -ForegroundColor Red
    $ErrorCount++
} else {
    Write-Host "✓ Restore successful" -ForegroundColor Green
}
Write-Host ""

# Step 3: Build solution
Write-Host "[3/5] Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed - DO NOT PUSH!" -ForegroundColor Red
    $ErrorCount++
} else {
    Write-Host "✓ Build successful" -ForegroundColor Green
}
Write-Host ""

# Step 4: Run tests (if any exist)
Write-Host "[4/5] Running tests..." -ForegroundColor Yellow
dotnet test --configuration Release --no-build --verbosity quiet 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠ No tests found or tests failed" -ForegroundColor Yellow
} else {
    Write-Host "✓ Tests passed" -ForegroundColor Green
}
Write-Host ""

# Step 5: Check for uncommitted changes
Write-Host "[5/5] Checking for uncommitted changes..." -ForegroundColor Yellow
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Host "⚠ You have uncommitted changes:" -ForegroundColor Yellow
    git status --short
    Write-Host ""
} else {
    Write-Host "✓ No uncommitted changes" -ForegroundColor Green
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
if ($ErrorCount -eq 0) {
    Write-Host "✓ ALL CHECKS PASSED - Safe to push!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "To push to production, run:" -ForegroundColor Cyan
    Write-Host "  git push origin main" -ForegroundColor White
    exit 0
} else {
    Write-Host "✗ $ErrorCount ERROR(S) FOUND - DO NOT PUSH!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Fix the errors above before pushing." -ForegroundColor Yellow
    exit 1
}
