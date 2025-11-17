using System;
using UMS.Core.Entities.Base;

namespace UMS.Core.Entities.Tenants
{
    /// <summary>
    /// SuperAdmin entity for system-wide administration
    /// Stored in Master Database only, not in tenant databases
    /// </summary>
    public class SuperAdmin : BaseEntity
    {
        /// <summary>
        /// SuperAdmin email address (unique)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// BCrypt hashed password
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the SuperAdmin
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Contact phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Whether the SuperAdmin account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Last successful login timestamp
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Email confirmation status
        /// </summary>
        public bool EmailConfirmed { get; set; } = true;

        /// <summary>
        /// Optional notes about this SuperAdmin account
        /// </summary>
        public string? Notes { get; set; }
    }
}
