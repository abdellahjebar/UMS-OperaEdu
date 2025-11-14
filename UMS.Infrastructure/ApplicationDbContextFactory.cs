using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            // Use the connection string from args if provided, otherwise use a default
            var connectionString = args.Length > 0 
                ? args[0] 
                : "Server=localhost;Database=UMS_Tenant_testuniversity;Trusted_Connection=True;TrustServerCertificate=True;";
            
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
