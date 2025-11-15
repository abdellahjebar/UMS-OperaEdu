using UMS.Core.Enums;

namespace UMS.Application.DTOs.Faculty
{
    public class FacultyDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? EmployeeNumber { get; set; }
        public Guid DepartmentId { get; set; }
        public FacultyTitle Title { get; set; }
        public DateTime HireDate { get; set; }
        public string? OfficeLocation { get; set; }
        public string? OfficeHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
