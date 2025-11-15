namespace UMS.Core.Interfaces
{
    /// <summary>
    /// Service to resolve and manage current tenant context
    /// </summary>
    public interface ITenantService
    {
        string? GetCurrentTenantId();
        string? GetTenantConnectionString();
        bool IsSuperAdmin();
        void SetTenantContext(string tenantId, string connectionString);
        void SetSuperAdminContext();
    }
}
