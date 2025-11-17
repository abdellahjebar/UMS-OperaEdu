using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure
{
    public class MasterDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
    {
        public MasterDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();
            
            // Master database connection string
            var connectionString = args.Length > 0 
                ? args[0] 
                : "Server=(localdb)\\mssqllocaldb;Database=UMS_Master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            
            optionsBuilder.UseSqlServer(connectionString);

            return new MasterDbContext(optionsBuilder.Options);
        }
    }
}
