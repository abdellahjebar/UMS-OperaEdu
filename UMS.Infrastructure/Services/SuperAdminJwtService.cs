using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UMS.Core.Entities.Tenants;
using UMS.Core.Interfaces;
using UMS.Core.Settings;

namespace UMS.Infrastructure.Services
{
    /// <summary>
    /// JWT service specifically for SuperAdmin authentication
    /// SuperAdmin tokens are different from tenant user tokens - they don't have a TenantId
    /// </summary>
    public class SuperAdminJwtService : ISuperAdminJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public SuperAdminJwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateSuperAdminToken(SuperAdmin superAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, superAdmin.Id.ToString()),
                new Claim(ClaimTypes.Email, superAdmin.Email),
                new Claim(ClaimTypes.Name, superAdmin.FullName),
                new Claim(ClaimTypes.Role, "SuperAdmin"),
                new Claim("IsSuperAdmin", "true"),
                new Claim("SuperAdminId", superAdmin.Id.ToString())
                // Note: No TenantId claim for SuperAdmin
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string? GetSuperAdminIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst("SuperAdminId")?.Value;
        }
    }
}
