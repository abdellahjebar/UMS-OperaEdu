using UMS.Core.Enums;

namespace UMS.Application.DTOs.Programs
{
    public class ProgramDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DegreeType DegreeType { get; set; }
        public Guid DepartmentId { get; set; }
        public int RequiredCredits { get; set; }
        public int DurationYears { get; set; }
        public string? Description { get; set; }
    }
}
