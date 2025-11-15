using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UMS.Core.Entities.Tenants;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistance.Seeders
{
    /// <summary>
    /// Seeds the master database with initial super-admin tenant and configuration
    /// </summary>
    public class MasterDbSeeder
    {
        private readonly MasterDbContext _context;
        private readonly ILogger<MasterDbSeeder> _logger;

        public MasterDbSeeder(MasterDbContext context, ILogger<MasterDbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the master database with default tenants
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                // Check if any tenants already exist
                if (await _context.Tenants.AnyAsync())
                {
                    _logger.LogInformation("Master database already contains tenant data. Skipping seed.");
                    return;
                }

                _logger.LogInformation("Seeding master database with initial tenant data...");

                // Create demo university tenant
                var demoTenant = new Tenant
                {
                    Name = "Demo University",
                    Subdomain = "demo",
                    ConnectionString = GetTenantConnectionString("demo"),
                    IsActive = true,
                    SubscriptionStartDate = DateTime.UtcNow,
                    SubscriptionEndDate = DateTime.UtcNow.AddYears(1),
                    AnnualFee = 10000m,
                    AdminEmail = "admin@demouniversity.edu",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tenants.Add(demoTenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded master database with {Count} tenant(s).", 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the master database.");
                throw;
            }
        }

        /// <summary>
        /// Generates a connection string for a tenant database
        /// In production, this should be read from configuration or environment variables
        /// </summary>
        private static string GetTenantConnectionString(string subdomain)
        {
            // This is a simplified approach - in production, you might want to:
            // 1. Read base connection string from configuration
            // 2. Support different database providers
            // 3. Use Azure SQL, AWS RDS, or other managed database services
            
            // For development with SQL Server
            return $"Server=(localdb)\\mssqllocaldb;Database=UMS_Tenant_{subdomain};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            
            // For PostgreSQL (uncomment if using PostgreSQL)
            // return $"Host=localhost;Database=ums_tenant_{subdomain};Username=postgres;Password=your_password";
        }
    }
}
