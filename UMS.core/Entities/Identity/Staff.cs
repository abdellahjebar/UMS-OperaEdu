using System;

namespace UMS.Core.Entities.Identity
{
    public class Staff : User
    {
        public string? EmployeeNumber { get; set; }
        public Guid DepartmentId { get; set; }
        public string? JobTitle { get; set; }
        public DateTime HireDate { get; set; }
    }
}