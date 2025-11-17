using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UMS.Core.Entities.Tenants;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.IRepositories;

namespace UMS.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IDatabaseInitializationService _databaseInitService;

        public CreateTenantCommandHandler(
            ITenantRepository tenantRepository,
            IDatabaseInitializationService databaseInitService)
        {
            _tenantRepository = tenantRepository;
            _databaseInitService = databaseInitService;
        }

        public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            // Validate subdomain is available
            if (await _tenantRepository.SubdomainExistsAsync(request.Subdomain))
            {
                throw new InvalidOperationException($"Subdomain '{request.Subdomain}' is already taken.");
            }

            // Connection string for LocalDB (development)
            // Format: Server=(localdb)\mssqllocaldb;Database=UMS_Tenant_{subdomain};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;
            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database=UMS_Tenant_{request.Subdomain};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

            // Create tenant entity
            var tenant = new Tenant
            {
                Name = request.Name,
                Subdomain = request.Subdomain.ToLower(),
                ConnectionString = connectionString,
                IsActive = true,
                SubscriptionStartDate = DateTime.UtcNow,
                SubscriptionEndDate = request.SubscriptionEndDate,
                AnnualFee = request.AnnualFee,
                AdminEmail = request.AdminEmail,
                AdminPhone = request.AdminPhone,
                Address = request.Address,
                MaxStudents = request.MaxStudents,
                MaxFaculty = request.MaxFaculty,
                MaxCourses = request.MaxCourses
            };

            // Save tenant to master database
            await _tenantRepository.AddAsync(tenant);

            // Create and initialize the tenant database with all migrations
            await _databaseInitService.EnsureDatabaseCreatedAsync(tenant);

            return tenant.Id;
        }
    }
}
