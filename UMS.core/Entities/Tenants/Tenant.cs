using System;
using UMS.Core.Entities.Base;

namespace UMS.Core.Entities.Tenants
{
    /// <summary>
    /// Represents a school/university in the multi-tenant system
    /// Each tenant gets its own isolated database
    /// </summary>
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., "Harvard University"
        public string Subdomain { get; set; } = string.Empty; // e.g., "harvard"
        public string ConnectionString { get; set; } = string.Empty; // Connection to tenant-specific database
        public bool IsActive { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public decimal AnnualFee { get; set; }
        public string? AdminEmail { get; set; }
        public string? AdminPhone { get; set; }
        public string? Address { get; set; }
        public string? LogoUrl { get; set; }
        
        // Configuration
        public int MaxStudents { get; set; } = 10000;
        public int MaxFaculty { get; set; } = 500;
        public int MaxCourses { get; set; } = 1000;

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool IsSubscriptionActive()
        {
            if (!IsActive) return false;
            if (SubscriptionEndDate.HasValue && SubscriptionEndDate.Value < DateTime.UtcNow)
                return false;
            return true;
        }
    }
}
