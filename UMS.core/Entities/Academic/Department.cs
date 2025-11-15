using System;
using System.Collections.Generic;
using UMS.Core.Entities.Base;

namespace UMS.Core.Entities.Academic
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? HeadOfDepartmentId { get; set; }
        public string? Building { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        // Navigation properties
        public virtual ICollection<Program> Programs { get; set; } = new List<Program>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
