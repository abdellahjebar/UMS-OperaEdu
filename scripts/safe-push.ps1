# Safe Push Script
# Validates everything, then pushes if all checks pass

param(
    [string]$CommitMessage = ""
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SAFE PUSH TO PRODUCTION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Change to solution directory
Set-Location $PSScriptRoot\..

# Run pre-push checks
Write-Host "Running pre-push validation..." -ForegroundColor Cyan
& "$PSScriptRoot\pre-push-check.ps1"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Validation failed. Aborting push." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

# Check if there are changes to commit
$gitStatus = git status --porcelain
if ($gitStatus) {
    if ([string]::IsNullOrWhiteSpace($CommitMessage)) {
        Write-Host "Enter commit message:" -ForegroundColor Yellow
        $CommitMessage = Read-Host
    }
    
    if ([string]::IsNullOrWhiteSpace($CommitMessage)) {
        Write-Host "✗ Commit message is required" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "Staging changes..." -ForegroundColor Yellow
    git add .
    
    Write-Host "Committing changes..." -ForegroundColor Yellow
    git commit -m $CommitMessage
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Commit failed" -ForegroundColor Red
        exit 1
    }
}

# Push to production
Write-Host ""
Write-Host "Pushing to production (origin/main)..." -ForegroundColor Yellow
git push origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "✓ SUCCESSFULLY PUSHED TO PRODUCTION!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Railway will automatically deploy your changes." -ForegroundColor Cyan
    Write-Host "Check deployment status at: https://railway.app" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "✗ Push failed" -ForegroundColor Red
    exit 1
}
