using System;

namespace UMS.Application.DTOs.Tenants
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public decimal AnnualFee { get; set; }
        public string? AdminEmail { get; set; }
        public string? AdminPhone { get; set; }
        public string? Address { get; set; }
        public int MaxStudents { get; set; }
        public int MaxFaculty { get; set; }
        public int MaxCourses { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
