using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UMS.Core.Interfaces;

namespace UMS.Application.Features.SuperAdmin.Commands.Login
{
    /// <summary>
    /// Handler for SuperAdmin login command
    /// Validates credentials against Master database and generates JWT token
    /// </summary>
    public class SuperAdminLoginCommandHandler : IRequestHandler<SuperAdminLoginCommand, SuperAdminLoginResponse>
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        private readonly ISuperAdminJwtService _jwtService;

        public SuperAdminLoginCommandHandler(
            ISuperAdminRepository superAdminRepository,
            ISuperAdminJwtService jwtService)
        {
            _superAdminRepository = superAdminRepository;
            _jwtService = jwtService;
        }

        public async Task<SuperAdminLoginResponse> Handle(SuperAdminLoginCommand request, CancellationToken cancellationToken)
        {
            // Validate credentials
            var superAdmin = await _superAdminRepository.ValidateCredentialsAsync(request.Email, request.Password);

            if (superAdmin == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Check if account is active
            if (!superAdmin.IsActive)
            {
                throw new UnauthorizedAccessException("Your SuperAdmin account has been deactivated. Please contact system support.");
            }

            // Generate JWT token with SuperAdmin claims
            var token = _jwtService.GenerateSuperAdminToken(superAdmin);

            // Update last login timestamp
            await _superAdminRepository.UpdateLastLoginAsync(superAdmin.Id);

            return new SuperAdminLoginResponse(
                Token: token,
                Email: superAdmin.Email,
                FullName: superAdmin.FullName,
                Message: "SuperAdmin login successful"
            );
        }
    }
}
