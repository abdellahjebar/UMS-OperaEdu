using System.Collections.Generic;
using System.Security.Claims;
using UMS.Core.Entities.Tenants;

namespace UMS.Core.Interfaces
{
    /// <summary>
    /// Interface for SuperAdmin authentication and JWT token generation
    /// </summary>
    public interface ISuperAdminJwtService
    {
        /// <summary>
        /// Generates JWT access token for SuperAdmin with appropriate claims
        /// </summary>
        string GenerateSuperAdminToken(SuperAdmin superAdmin);

        /// <summary>
        /// Validates SuperAdmin JWT token and returns claims principal
        /// </summary>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Extracts SuperAdmin ID from JWT token
        /// </summary>
        string? GetSuperAdminIdFromToken(string token);
    }
}
