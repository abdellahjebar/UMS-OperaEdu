using UMS.Core.Enums;

namespace UMS.Application.DTOs.Sections
{
    public class SectionDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string SectionNumber { get; set; } = string.Empty;
        public Term Term { get; set; }
        public int Year { get; set; }
        public Guid InstructorId { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
