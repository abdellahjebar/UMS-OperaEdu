using System;
using UMS.Core.Enums;

namespace UMS.Application.DTOs.Students
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? StudentNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ExpectedGraduationDate { get; set; }
        public Guid ProgramId { get; set; }
        public string? ProgramName { get; set; }  // Program name for display
        public AcademicStatus AcademicStatus { get; set; }
        public decimal GPA { get; set; }
        public int TotalCredits { get; set; }
        public bool IsActive { get; set; }
    }
}
