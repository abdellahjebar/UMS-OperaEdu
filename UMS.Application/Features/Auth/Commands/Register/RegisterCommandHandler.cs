using MediatR;
using UMS.Application.DTOs.Auth;
using UMS.Core.Entities.Identity;
using UMS.Core.Enums;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.IRepositories;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ITenantService _tenantService;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            ITenantService tenantService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _tenantService = tenantService;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var emailExists = await _userRepository.EmailExistsAsync(request.Email);

            if (emailExists)
            {
                throw new BadRequestException("User with this email already exists");
            }

            // Parse user type
            if (!Enum.TryParse<UserType>(request.UserType, out var userType))
            {
                throw new BadRequestException("Invalid user type");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user based on type
            User user = userType switch
            {
                UserType.Student => new Student
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    UserType = UserType.Student,
                    StudentNumber = request.StudentId ?? GenerateStudentId(),
                    ProgramId = request.ProgramId ?? Guid.Empty,
                    EnrollmentDate = DateTime.UtcNow,
                    AcademicStatus = AcademicStatus.Active,
                    GPA = 0.0m,
                    TotalCredits = 0
                },
                UserType.Faculty => new Faculty
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    UserType = UserType.Faculty
                },
                UserType.Staff => new Staff
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    UserType = UserType.Staff
                },
                _ => throw new BadRequestException("Unsupported user type for registration")
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Get tenant ID
            var tenantId = _tenantService.GetCurrentTenantId();
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedException("No tenant context available");
            }

            // Determine roles
            var roles = GetUserRoles(userType.ToString());

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
                UserType = userType.ToString(),
                Roles = roles.ToList()
            };
        }

        private string GenerateStudentId()
        {
            // Generate a unique student ID (format: STU + timestamp)
            return $"STU{DateTime.UtcNow:yyyyMMddHHmmss}";
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
