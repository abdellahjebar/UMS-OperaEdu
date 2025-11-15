using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UMS.Core.Entities.Tenants;
using UMS.Core.Interfaces;
using UMS.Infrastructure.Persistance.Seeders;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Services
{
    public class DatabaseInitializationService : IDatabaseInitializationService
    {
        private readonly ILogger<DatabaseInitializationService> _logger;
        private readonly IConfiguration _configuration;

        public DatabaseInitializationService(
            ILogger<DatabaseInitializationService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task EnsureDatabaseCreatedAsync(Tenant tenant)
        {
            try
            {
                _logger.LogInformation("Checking database for tenant: {TenantName} (Subdomain: {Subdomain})", 
                    tenant.Name, tenant.Subdomain);

                // Create DbContext with tenant's connection string
                var dbProvider = _configuration["DatabaseProvider"] ?? "SqlServer";
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                
                if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                {
                    optionsBuilder.UseNpgsql(tenant.ConnectionString);
                }
                else
                {
                    optionsBuilder.UseSqlServer(tenant.ConnectionString);
                }

                using var context = new ApplicationDbContext(optionsBuilder.Options);

                // Check if database exists
                var canConnect = await context.Database.CanConnectAsync();
                bool isNewDatabase = false;

                if (!canConnect)
                {
                    _logger.LogInformation("Database does not exist. Creating database for tenant: {TenantName}", tenant.Name);

                    // Create database and apply all migrations
                    await context.Database.MigrateAsync();
                    isNewDatabase = true;

                    _logger.LogInformation("Database created successfully for tenant: {TenantName}", tenant.Name);
                }
                else
                {
                    _logger.LogInformation("Database already exists for tenant: {TenantName}. Checking for pending migrations...", tenant.Name);

                    // Check for pending migrations
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    
                    if (pendingMigrations.Any())
                    {
                        _logger.LogInformation("Applying {Count} pending migrations for tenant: {TenantName}", 
                            pendingMigrations.Count(), tenant.Name);
                        
                        await context.Database.MigrateAsync();
                        
                        _logger.LogInformation("Migrations applied successfully for tenant: {TenantName}", tenant.Name);
                    }
                    else
                    {
                        _logger.LogInformation("Database is up to date for tenant: {TenantName}", tenant.Name);
                    }
                }

                // Seed database if it's newly created or empty
                if (isNewDatabase || !await context.Users.AnyAsync())
                {
                    _logger.LogInformation("Seeding database for tenant: {TenantName}", tenant.Name);
                    var seederLogger = new LoggerFactory().CreateLogger<TenantDbSeeder>();
                    var seeder = new TenantDbSeeder(context, seederLogger);
                    await seeder.SeedAsync();
                    _logger.LogInformation("Database seeded successfully for tenant: {TenantName}", tenant.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring database created for tenant: {TenantName}", tenant.Name);
                throw;
            }
        }

        public async Task<bool> DatabaseExistsAsync(string connectionString)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using var context = new ApplicationDbContext(optionsBuilder.Options);
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}
