using System;
using UMS.Core.Enums;

namespace UMS.Core.Entities.Identity
{
    public class Faculty : User
    {
        public string? EmployeeNumber { get; set; }
        public Guid DepartmentId { get; set; }
        public FacultyTitle Title { get; set; }
        public DateTime HireDate { get; set; }
        public string? OfficeLocation { get; set; }
        public string? OfficeHours { get; set ; }
    }
}