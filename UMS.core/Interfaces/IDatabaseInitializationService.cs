using UMS.Core.Entities.Tenants;

namespace UMS.Core.Interfaces
{
    public interface IDatabaseInitializationService
    {
        Task EnsureDatabaseCreatedAsync(Tenant tenant);
        Task<bool> DatabaseExistsAsync(string connectionString);
    }
}
