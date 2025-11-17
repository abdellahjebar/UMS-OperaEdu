using Microsoft.EntityFrameworkCore;
using UMS.Core.Entities.Tenants;

namespace UMS.Infrastructure.Persistence
{
    /// <summary>
    /// Master database context for managing tenants
    /// This database stores tenant information and connection strings
    /// Each tenant's actual data is in their own separate database
    /// </summary>
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Configurations.TenantConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SuperAdminConfiguration());
        }
    }
}
