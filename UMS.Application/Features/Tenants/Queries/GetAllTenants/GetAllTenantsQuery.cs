using MediatR;
using System.Collections.Generic;
using UMS.Application.DTOs.Tenants;

namespace UMS.Application.Features.Tenants.Queries.GetAllTenants
{
    public class GetAllTenantsQuery : IRequest<IEnumerable<TenantDto>>
    {
    }
}
