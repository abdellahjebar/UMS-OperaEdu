using System;
using System.Threading.Tasks;
using UMS.Core.Entities.Tenants;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Core.Interfaces.IRepositories
{
    /// <summary>
    /// Repository for managing tenants (used by super admin only)
    /// This repository works on the MASTER database, not tenant databases
    /// </summary>
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant?> GetBySubdomainAsync(string subdomain);
        Task<bool> SubdomainExistsAsync(string subdomain);
        Task<bool> IsConnectionStringValidAsync(string connectionString);
    }
}
