using MediatR;
using UMS.Application.DTOs.Auth;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ITenantService _tenantService;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IJwtService jwtService,
            ITenantService tenantService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _tenantService = tenantService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Get current tenant ID
            var tenantId = _tenantService.GetCurrentTenantId();
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedException("No tenant context available");
            }

            // Determine roles based on user type
            var roles = GetUserRoles(user.UserType.ToString());

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user, tenantId, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                FullName = $"{user.FirstName} {user.LastName}",
                UserType = user.UserType.ToString(),
                Roles = roles.ToList()
            };
        }

        private IEnumerable<string> GetUserRoles(string userType)
        {
            return userType switch
            {
                "Student" => new[] { "Student" },
                "Faculty" => new[] { "Faculty", "Instructor" },
                "Staff" => new[] { "Staff" },
                "Admin" => new[] { "Admin", "Staff" },
                _ => new[] { "User" }
            };
        }
    }
}
