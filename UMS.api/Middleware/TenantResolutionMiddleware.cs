using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UMS.API.Models;
using UMS.Core.Interfaces;
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
        private readonly ILogger<TenantResolutionMiddleware> _logger;
        private const string SUPER_ADMIN_SUBDOMAIN = "admin"; // admin.yourdomain.com

        public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantRepository tenantRepository, ITenantService tenantService, UMS.Core.Interfaces.IDatabaseInitializationService databaseInitService)
        {
            var host = context.Request.Host.Host;
            var subdomain = ExtractSubdomain(host);

            if (string.IsNullOrEmpty(subdomain))
            {
                _logger.LogWarning("Tenant resolution failed: No subdomain found in host {Host}", host);
                await WriteErrorResponse(context, StatusCodes.Status400BadRequest, 
                    "Invalid request", 
                    "No subdomain found. Please access via subdomain (e.g., schoolname.domain.com)");
                return;
            }

            // Check if super admin
            if (subdomain.Equals(SUPER_ADMIN_SUBDOMAIN, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Super admin context activated for host {Host}", host);
                tenantService.SetSuperAdminContext();
                await _next(context);
                return;
            }

            // Resolve tenant
            var tenant = await tenantRepository.GetBySubdomainAsync(subdomain);

            if (tenant == null)
            {
                _logger.LogWarning("Tenant resolution failed: Subdomain {Subdomain} not found", subdomain);
                await WriteErrorResponse(context, StatusCodes.Status404NotFound, 
                    "Tenant not found", 
                    $"School '{subdomain}' not found. Please contact administrator.");
                return;
            }

            if (!tenant.IsSubscriptionActive())
            {
                _logger.LogWarning("Tenant access denied: Subscription inactive for tenant {TenantId} (subdomain: {Subdomain})", 
                    tenant.Id, subdomain);
                await WriteErrorResponse(context, StatusCodes.Status403Forbidden, 
                    "Subscription inactive", 
                    "School subscription is not active. Please contact administrator.");
                return;
            }

            _logger.LogDebug("Tenant resolved: TenantId={TenantId}, Subdomain={Subdomain}, Name={TenantName}", 
                tenant.Id, subdomain, tenant.Name);

            // Ensure database exists and is up to date
            await databaseInitService.EnsureDatabaseCreatedAsync(tenant);

            // Set tenant context
            tenantService.SetTenantContext(tenant.Id.ToString(), tenant.ConnectionString);

            await _next(context);
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string title, string detail)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                StatusCode = statusCode,
                Title = title,
                Detail = detail,
                TraceId = context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
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
