using System;
using UMS.Core.Entities.Base;

namespace UMS.Core.Entities.Academic
{
    public class Course : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public Guid DepartmentId { get; set; }
    }
}