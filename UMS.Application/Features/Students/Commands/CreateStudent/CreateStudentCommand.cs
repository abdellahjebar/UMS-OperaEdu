using MediatR;
using System;
using UMS.Core.Enums;

namespace UMS.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommand : IRequest<Guid>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? StudentNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ExpectedGraduationDate { get; set; }
        public Guid ProgramId { get; set; }
    }
}
