using System;
using UMS.Core.Entities.Base;
using UMS.Core.Enums;

namespace UMS.Core.Entities.Academic
{
    public class Program : BaseEntity
    {
        public string? Name { get; set; }
        public string? Code { get; set; } 
        public DegreeType DegreeType { get; set; }
        public Guid DepartmentId { get; set; }
        public int RequiredCredits { get; set; }
        public int DurationYears { get; set; }
        public string? Description { get; set; } 
    }
}