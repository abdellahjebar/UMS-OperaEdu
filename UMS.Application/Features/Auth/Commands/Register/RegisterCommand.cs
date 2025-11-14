using MediatR;
using UMS.Application.DTOs.Auth;

namespace UMS.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserType { get; set; } = "Student"; // Default to Student
        
        // Student-specific fields (optional)
        public string? StudentId { get; set; }
        public Guid? ProgramId { get; set; }
    }
}
