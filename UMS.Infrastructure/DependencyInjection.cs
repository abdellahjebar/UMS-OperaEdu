using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.IRepositories;
using UMS.Core.Interfaces.Repositories;
using UMS.Core.Settings;
using UMS.Infrastructure.Persistence;
using UMS.Infrastructure.Persistence.Repositories;
using UMS.Infrastructure.Services;

namespace UMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Master Database Context (for tenant management)
            var masterConnection = configuration.GetConnectionString("MasterConnection");
            var dbProvider = configuration["DatabaseProvider"] ?? "SqlServer"; // Default to SQL Server
            
            services.AddDbContext<MasterDbContext>(options =>
            {
                if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseNpgsql(masterConnection,
                        b => b.MigrationsAssembly(typeof(MasterDbContext).Assembly.FullName));
                }
                else
                {
                    options.UseSqlServer(masterConnection,
                        b => b.MigrationsAssembly(typeof(MasterDbContext).Assembly.FullName));
                }
            });

            // Tenant Service
            services.AddScoped<TenantService>();
            services.AddScoped<ITenantService>(sp => sp.GetRequiredService<TenantService>());

            // Tenant Database Context (dynamically resolved per request)
            services.AddScoped<ApplicationDbContext>(sp =>
            {
                var tenantService = sp.GetRequiredService<ITenantService>();
                var connectionString = tenantService.GetTenantConnectionString();
                var tenantOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

                if (string.IsNullOrEmpty(connectionString))
                {
                    // If no tenant context (e.g., super admin accessing tenant management)
                    // Return a context that won't be used
                    return new ApplicationDbContext(tenantOptionsBuilder.Options);
                }
                
                if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                {
                    tenantOptionsBuilder.UseNpgsql(connectionString);
                }
                else
                {
                    tenantOptionsBuilder.UseSqlServer(connectionString);
                }

                return new ApplicationDbContext(tenantOptionsBuilder.Options);
            });

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // JWT Service
            services.AddScoped<IJwtService, JwtService>();

            // Database Initialization Service
            services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();

            return services;
        }
    }
}