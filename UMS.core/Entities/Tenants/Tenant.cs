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
        
        // Academic Configuration
        public Enums.GradingSystem PreferredGradingSystem { get; set; } = Enums.GradingSystem.French; // Default to French 0-20 scale
        public int CreditsPerYear { get; set; } = 60; // ECTS standard: 60 credits per academic year
        public string? AcademicYearFormat { get; set; } = "S{0}"; // e.g., "S1", "S2" for French semesters

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
