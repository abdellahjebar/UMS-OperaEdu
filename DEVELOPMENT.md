# Development Workflow Guide

This guide helps you develop, test, and deploy changes safely.

---

## 🔄 Development Workflow

### 1. Make Your Changes
Edit files in:
- `UMS.Core/` - Domain entities, interfaces
- `UMS.Application/` - Business logic, CQRS handlers
- `UMS.Infrastructure/` - Database, repositories
- `UMS.API/` - Controllers, middleware

### 2. Test Locally
```powershell
# Build and run API for testing
.\scripts\test-locally.ps1
```

The API will start at:
- **API**: http://localhost:5263
- **Swagger UI**: http://localhost:5263/swagger

**Test your changes with Postman/Swagger:**
- Test authentication endpoints
- Test your new features
- Verify database operations work

Press `CTRL+C` to stop the API when done.

### 3. Validate Before Commit
```powershell
# Run all validation checks
.\scripts\pre-push-check.ps1
```

This validates:
- ✓ Clean build succeeds
- ✓ No compilation errors
- ✓ Dependencies restore correctly
- ✓ Tests pass (if any)

### 4. Commit Your Changes
If validation passes:
```powershell
git add .
git commit -m "Your descriptive commit message"
```

### 5. Push to Production
```powershell
# Safe push with validation
.\scripts\safe-push.ps1 -CommitMessage "Your commit message"
```

OR manually:
```powershell
# Validate one more time
.\scripts\pre-push-check.ps1

# If all checks pass, push
git push origin main
```

---

## 🧪 Testing Scenarios

### Test New API Endpoint
1. Start local API: `.\scripts\test-locally.ps1`
2. Open Swagger: http://localhost:5263/swagger
3. Test your endpoint
4. Check database changes in SQL Server Management Studio
5. Stop API (CTRL+C)

### Test Database Changes
1. Make migration changes
2. Start API (applies migrations automatically)
3. Verify tables/columns in SSMS
4. Test CRUD operations via Postman
5. Check for errors in API logs

### Test Authentication Flow
1. Start API
2. POST `/api/auth/register` - Create user
3. POST `/api/auth/login` - Get JWT token
4. GET protected endpoint with Authorization header
5. Verify token validation works

### Test Multi-Tenancy
1. Create tenant: POST `http://admin.localhost:5263/api/tenants`
2. Verify database created in SSMS (UMS_Tenant_xxx)
3. Register user on tenant: `http://<subdomain>.localhost:5263/api/auth/register`
4. Verify user in tenant database
5. Test tenant isolation

---

## 🚨 Common Issues & Fixes

### Build Fails Locally
```powershell
# Clean everything
dotnet clean
rm -r */bin, */obj -Force

# Restore and rebuild
dotnet restore
dotnet build
```

### API Won't Start
```powershell
# Check if port 5263 is in use
Get-NetTCPConnection -LocalPort 5263

# Kill any process using the port
Stop-Process -Id <PID>

# Or use different port in launchSettings.json
```

### Database Connection Error
- Verify SQL Server is running
- Check connection string in `appsettings.Development.json`
- Ensure database exists (API creates it on first run)

### Changes Not Reflecting
```powershell
# Stop the API (CTRL+C)
# Rebuild
dotnet build

# Restart API
.\scripts\test-locally.ps1
```

---

## 📋 Quick Reference

### Essential Commands
```powershell
# Test locally with auto-run
.\scripts\test-locally.ps1

# Test locally (build only, no API start)
.\scripts\test-locally.ps1 -SkipApi

# Validate before pushing
.\scripts\pre-push-check.ps1

# Safe push to production
.\scripts\safe-push.ps1

# Manual push (after validation)
git push origin main
```

### File Locations
- **Scripts**: `scripts/`
- **Configuration**: `UMS.api/appsettings.json`
- **Database Context**: `UMS.Infrastructure/Persistence/`
- **Migrations**: `UMS.Infrastructure/Migrations/`

---

## 🔐 Environment Configuration

### Local Development (SQL Server)
```json
// appsettings.Development.json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "MasterConnection": "Server=localhost;Database=UMS_Master;..."
  }
}
```

### Production (Railway - PostgreSQL)
Environment variables in Railway:
```
DatabaseProvider=PostgreSQL
ConnectionStrings__MasterConnection=<railway-postgres-url>
```

---

## ✅ Checklist Before Pushing

- [ ] Code builds successfully (`.\scripts\test-locally.ps1 -SkipApi`)
- [ ] API starts without errors (`.\scripts\test-locally.ps1`)
- [ ] Tested new features via Swagger/Postman
- [ ] Database operations work correctly
- [ ] No breaking changes to existing features
- [ ] Validation passes (`.\scripts\pre-push-check.ps1`)
- [ ] Committed with descriptive message
- [ ] Ready to push (`.\scripts\safe-push.ps1`)

---

## 🎯 Best Practices

1. **Always test locally first** - Don't push untested code
2. **Use descriptive commit messages** - Explain what and why
3. **Test edge cases** - Try to break your own code
4. **Check database** - Verify migrations and data integrity
5. **Run validation** - Use pre-push-check before every push
6. **Small commits** - Easier to debug if something breaks
7. **Test both DB providers** - SQL Server locally, PostgreSQL for production

---

## 📞 Need Help?

- **GitHub Issues**: https://github.com/abdellahjebar/UMS-OperaEdu/issues
- **Documentation**: Check README.md and DEPLOYMENT.md
- **Logs**: Check console output and Railway logs

---

**Remember**: It's faster to test locally for 5 minutes than to debug a broken production deployment for 30 minutes! 🚀
