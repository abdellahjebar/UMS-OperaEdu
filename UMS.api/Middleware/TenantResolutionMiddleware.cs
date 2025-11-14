using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using UMS.Core.Interfaces.IRepositories;
using UMS.Infrastructure.Services;

namespace UMS.API.Middleware
{
    /// <summary>
    /// Middleware to resolve tenant from subdomain and set tenant context
    /// Example: harvard.yourdomain.com -> resolves "harvard" tenant
    /// </summary>
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private const string SUPER_ADMIN_SUBDOMAIN = "admin"; // admin.yourdomain.com

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantRepository tenantRepository, TenantService tenantService, UMS.Core.Interfaces.IDatabaseInitializationService databaseInitService)
        {
            var host = context.Request.Host.Host;
            var subdomain = ExtractSubdomain(host);

            if (string.IsNullOrEmpty(subdomain))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = 400,
                    Message = "Invalid request. No subdomain found. Please access via subdomain (e.g., schoolname.domain.com)"
                });
                return;
            }

            // Check if super admin
            if (subdomain.Equals(SUPER_ADMIN_SUBDOMAIN, StringComparison.OrdinalIgnoreCase))
            {
                tenantService.SetSuperAdminContext();
                await _next(context);
                return;
            }

            // Resolve tenant
            var tenant = await tenantRepository.GetBySubdomainAsync(subdomain);

            if (tenant == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = 404,
                    Message = $"School '{subdomain}' not found. Please contact administrator."
                });
                return;
            }

            if (!tenant.IsSubscriptionActive())
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = 403,
                    Message = "School subscription is not active. Please contact administrator."
                });
                return;
            }

            // Ensure database exists and is up to date
            await databaseInitService.EnsureDatabaseCreatedAsync(tenant);

            // Set tenant context
            tenantService.SetTenantContext(tenant.Id.ToString(), tenant.ConnectionString);

            await _next(context);
        }

        private string? ExtractSubdomain(string host)
        {
            // Remove port if exists
            if (host.Contains(':'))
            {
                host = host.Split(':')[0];
            }

            // For localhost testing, extract from format: localhost or schoolname.localhost
            if (host.Contains("localhost"))
            {
                var parts = host.Split('.');
                return parts.Length > 1 ? parts[0] : "admin"; // Default to admin for plain localhost
            }

            // Production: extract subdomain from host
            var hostParts = host.Split('.');
            
            // Need at least 3 parts: subdomain.domain.tld
            if (hostParts.Length >= 3)
            {
                return hostParts[0];
            }

            // If only 2 parts (domain.tld), no subdomain
            return null;
        }
    }
}
