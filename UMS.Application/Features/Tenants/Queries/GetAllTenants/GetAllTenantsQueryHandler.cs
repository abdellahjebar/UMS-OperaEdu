using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UMS.Application.DTOs.Tenants;
using UMS.Core.Interfaces.IRepositories;

namespace UMS.Application.Features.Tenants.Queries.GetAllTenants
{
    public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
    {
        private readonly ITenantRepository _tenantRepository;

        public GetAllTenantsQueryHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<IEnumerable<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _tenantRepository.GetAllAsync();

            return tenants.Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Subdomain = t.Subdomain,
                IsActive = t.IsActive,
                SubscriptionStartDate = t.SubscriptionStartDate,
                SubscriptionEndDate = t.SubscriptionEndDate,
                AnnualFee = t.AnnualFee,
                AdminEmail = t.AdminEmail,
                AdminPhone = t.AdminPhone,
                Address = t.Address,
                MaxStudents = t.MaxStudents,
                MaxFaculty = t.MaxFaculty,
                MaxCourses = t.MaxCourses,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
    }
}
