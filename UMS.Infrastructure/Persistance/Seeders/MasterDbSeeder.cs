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
        /// Seeds the master database with default tenants and SuperAdmin
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                // Seed SuperAdmin first
                await SeedSuperAdminAsync();

                // Check if any tenants already exist
                if (await _context.Tenants.AnyAsync())
                {
                    _logger.LogInformation("Master database already contains tenant data. Skipping tenant seed.");
                    return;
                }

                _logger.LogInformation("Seeding master database with initial tenant data...");

                // Create test university tenant
                var testTenant = new Tenant
                {
                    Name = "Test University",
                    Subdomain = "testuniversity",
                    ConnectionString = GetTenantConnectionString("testuniversity"),
                    IsActive = true,
                    SubscriptionStartDate = DateTime.UtcNow,
                    SubscriptionEndDate = DateTime.UtcNow.AddYears(1),
                    AnnualFee = 10000m,
                    AdminEmail = "admin@testuniversity.edu",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tenants.Add(testTenant);
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
        /// Seeds the default SuperAdmin account
        /// </summary>
        private async Task SeedSuperAdminAsync()
        {
            const string defaultEmail = "superadmin@umsoperaedu.com";

            // Check if SuperAdmin already exists
            if (await _context.SuperAdmins.AnyAsync(sa => sa.Email == defaultEmail))
            {
                _logger.LogInformation("SuperAdmin account already exists. Skipping SuperAdmin seed.");
                return;
            }

            _logger.LogInformation("Seeding default SuperAdmin account...");

            // Create default SuperAdmin
            // Default password: SuperAdmin@123 (should be changed after first login)
            var superAdmin = new SuperAdmin
            {
                Email = defaultEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"),
                FullName = "System Administrator",
                PhoneNumber = "+1-555-0100",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Notes = "Default SuperAdmin account created during initial seeding. Please change password after first login."
            };

            _context.SuperAdmins.Add(superAdmin);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded default SuperAdmin account: {Email}", defaultEmail);
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
