using Microsoft.AspNetCore.Http;
using System.Linq;
using UMS.Core.Interfaces;

namespace UMS.Infrastructure.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TENANT_ID_KEY = "TenantId";
        private const string TENANT_CONNECTION_KEY = "TenantConnection";
        private const string IS_SUPER_ADMIN_KEY = "IsSuperAdmin";

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentTenantId()
        {
            return _httpContextAccessor.HttpContext?.Items[TENANT_ID_KEY]?.ToString();
        }

        public string? GetTenantConnectionString()
        {
            return _httpContextAccessor.HttpContext?.Items[TENANT_CONNECTION_KEY]?.ToString();
        }

        public bool IsSuperAdmin()
        {
            var isSuperAdmin = _httpContextAccessor.HttpContext?.Items[IS_SUPER_ADMIN_KEY];
            return isSuperAdmin != null && (bool)isSuperAdmin;
        }

        public void SetTenantContext(string tenantId, string connectionString)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items[TENANT_ID_KEY] = tenantId;
                _httpContextAccessor.HttpContext.Items[TENANT_CONNECTION_KEY] = connectionString;
            }
        }

        public void SetSuperAdminContext()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items[IS_SUPER_ADMIN_KEY] = true;
            }
        }
    }
}
