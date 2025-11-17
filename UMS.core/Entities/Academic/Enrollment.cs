using System;
using UMS.Core.Entities.Base;
using UMS.Core.Entities.Identity;
using UMS.Core.Enums;

namespace UMS.Core.Entities.Academic
{
    public class Enrollment : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        
        // Grading fields
        public decimal? NumericGrade { get; set; }        // French: 0-20, American: 0-4.0, Percentage: 0-100
        public GradeType? LetterGrade { get; set; }       // Letter grade (A, B, C, etc.)
        public decimal? GradePoints { get; set; }         // Calculated grade points for GPA
        public string? GradeComments { get; set; }        // Optional instructor comments
        public DateTime? GradedAt { get; set; }           // When grade was assigned
        public Guid? GradedBy { get; set; }               // Faculty member who assigned grade

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Section Section { get; set; } = null!;
        public virtual Faculty? GradedByFaculty { get; set; }
    }
}