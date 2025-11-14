using System;
using System.Collections.Generic;
using UMS.Core.Enums;

namespace UMS.Core.Entities.Identity
{
    public class Student : User
    {
        public string? StudentNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ExpectedGraduationDate { get; set; }
        public Guid ProgramId { get; set; }
        public AcademicStatus AcademicStatus { get; set; }
        public decimal GPA { get; set; }
        public int TotalCredits { get; set; }
    }
}