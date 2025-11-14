using System;
using System.Collections.Generic;
using System.Security.Claims;
using UMS.Core.Entities.Identity;

namespace UMS.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, string tenantId, IEnumerable<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        string? GetUserIdFromToken(string token);
        string? GetTenantIdFromToken(string token);
    }
}
