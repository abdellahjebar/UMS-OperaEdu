# Deployment Guide

## Pre-Deployment Checklist

Before pushing to production, always run validation:

### Option 1: Manual Validation
```powershell
# Run pre-push checks
.\scripts\pre-push-check.ps1
```

This will:
- Clean previous builds
- Restore dependencies
- Build the solution in Release mode
- Run tests (if available)
- Check for uncommitted changes

### Option 2: Safe Push (Recommended)
```powershell
# Validate, commit, and push in one command
.\scripts\safe-push.ps1 -CommitMessage "Your commit message here"

# Or run interactively (will prompt for message)
.\scripts\safe-push.ps1
```

## Deployment Workflow

```
Local Development
    ↓
Run pre-push-check.ps1 ✓
    ↓
Fix any errors found
    ↓
Run safe-push.ps1
    ↓
GitHub (main branch)
    ↓
GitHub Actions CI/CD
    ↓
Railway Automatic Deployment
    ↓
Production 🚀
```

## Railway Configuration

### Environment Variables Required:
```
DatabaseProvider=PostgreSQL
ConnectionStrings__MasterConnection=<railway-postgres-connection-string>

JwtSettings__SecretKey=<your-secret-key-min-32-chars>
JwtSettings__Issuer=UMS-OperaEdu
JwtSettings__Audience=UMS-Users
JwtSettings__AccessTokenExpirationMinutes=30
JwtSettings__RefreshTokenExpirationDays=7

ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
```

### Getting PostgreSQL Connection String:
1. Go to Railway dashboard
2. Click on your PostgreSQL service
3. Go to "Variables" tab
4. Copy `DATABASE_URL` value
5. Add it as `ConnectionStrings__MasterConnection` in your API service

## Monitoring Deployment

### Check GitHub Actions:
https://github.com/abdellahjebar/UMS-OperaEdu/actions

### Check Railway Logs:
1. Go to Railway dashboard
2. Click on UMS-OperaEdu service
3. View "Deployments" tab
4. Click latest deployment to see logs

## Rollback Procedure

If deployment fails:

```powershell
# Revert to previous commit
git revert HEAD
git push origin main
```

Or in Railway:
1. Go to Deployments tab
2. Click on a previous successful deployment
3. Click "Redeploy"

## Local Testing Before Push

### Test with SQL Server (Local):
```powershell
cd UMS.api
dotnet run
```

### Test with PostgreSQL (Railway-like):
Update `appsettings.Development.json`:
```json
{
  "DatabaseProvider": "PostgreSQL",
  "ConnectionStrings": {
    "MasterConnection": "your-local-postgres-connection"
  }
}
```

## Best Practices

1. ✓ **Always run pre-push checks** before pushing
2. ✓ **Test locally first** with both SQL Server and PostgreSQL if possible
3. ✓ **Use descriptive commit messages**
4. ✓ **Check GitHub Actions** after pushing
5. ✓ **Monitor Railway deployment logs**
6. ✓ **Test API endpoints** after deployment
7. ✓ **Keep environment variables secure** (never commit them)

## Quick Commands

```powershell
# Validate only (no push)
.\scripts\pre-push-check.ps1

# Safe push with validation
.\scripts\safe-push.ps1 -CommitMessage "Add new feature"

# Manual push (after validation passes)
git add .
git commit -m "Your message"
git push origin main

# Check local build
dotnet build --configuration Release

# Run locally
cd UMS.api
dotnet run
```

## Troubleshooting

### Build fails locally:
- Run `dotnet clean`
- Run `dotnet restore`
- Check for syntax errors

### Build passes locally but fails on Railway:
- Check case-sensitive file paths (Railway uses Linux)
- Verify all package references are correct
- Check Dockerfile syntax

### Database connection fails:
- Verify `DATABASE_URL` in Railway PostgreSQL service
- Check `ConnectionStrings__MasterConnection` is set correctly
- Ensure `DatabaseProvider=PostgreSQL` is set

### API returns 500 errors:
- Check Railway logs for detailed error messages
- Verify all environment variables are set
- Check database migrations were applied
