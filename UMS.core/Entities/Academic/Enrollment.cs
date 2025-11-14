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
        public decimal? NumericGrade { get; set; }
        public decimal? GradePoints { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Section Section { get; set; } = null!;
    }
}