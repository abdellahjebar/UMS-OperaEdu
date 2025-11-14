using UMS.Core.Enums;

namespace UMS.Application.DTOs.Enrollments
{
    public class EnrollmentDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        public decimal? NumericGrade { get; set; }
        public decimal? GradePoints { get; set; }
    }
}
