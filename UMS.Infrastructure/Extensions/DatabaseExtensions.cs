using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UMS.Infrastructure.Persistance.Seeders;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for database initialization and seeding
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Applies migrations and seeds data for both master and tenant databases
        /// </summary>
        public static async Task InitializeDatabasesAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                // Initialize Master Database
                await InitializeMasterDatabaseAsync(services);

                // Note: Tenant databases are initialized on-demand when tenants are created
                // or when the first request for a tenant is received
            }
            catch (Exception ex)
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DatabaseInitialization");
                logger.LogError(ex, "An error occurred during database initialization.");
                throw;
            }
        }

        /// <summary>
        /// Initializes and seeds the master database
        /// </summary>
        private static async Task InitializeMasterDatabaseAsync(IServiceProvider services)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseInitialization");
            var masterContext = services.GetRequiredService<MasterDbContext>();

            logger.LogInformation("Initializing master database...");

            // Apply migrations
            var pendingMigrations = await masterContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations to master database...", pendingMigrations.Count());
                await masterContext.Database.MigrateAsync();
                logger.LogInformation("Master database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Master database is up to date.");
            }

            // Seed master database
            var masterSeeder = new MasterDbSeeder(masterContext, services.GetRequiredService<ILogger<MasterDbSeeder>>());
            await masterSeeder.SeedAsync();
        }
    }
}
