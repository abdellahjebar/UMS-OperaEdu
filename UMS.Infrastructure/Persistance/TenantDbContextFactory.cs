using Microsoft.EntityFrameworkCore;
using System;
using UMS.Core.Interfaces;

namespace UMS.Infrastructure.Persistence
{
    /// <summary>
    /// Factory to create ApplicationDbContext with tenant-specific connection string
    /// </summary>
    public class TenantDbContextFactory
    {
        private readonly ITenantService _tenantService;
        private readonly DbContextOptions<ApplicationDbContext> _baseOptions;

        public TenantDbContextFactory(ITenantService tenantService, DbContextOptions<ApplicationDbContext> baseOptions)
        {
            _tenantService = tenantService;
            _baseOptions = baseOptions;
        }

        public ApplicationDbContext CreateDbContext()
        {
            var connectionString = _tenantService.GetTenantConnectionString();

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Tenant connection string not found in context.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
