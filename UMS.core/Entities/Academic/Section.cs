using System;
using System.Collections.Generic;
using UMS.Core.Entities.Base;
using UMS.Core.Enums;

namespace UMS.Core.Entities.Academic
{
    public class Section : BaseEntity
    {
        public Guid CourseId { get; set; }
        public string SectionNumber { get; set; } = string.Empty;
        public Term Term { get; set; }
        public int Year { get; set; }
        public Guid InstructorId { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}