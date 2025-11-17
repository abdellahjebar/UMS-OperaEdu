using System;
using System.Threading.Tasks;
using UMS.Core.Entities.Tenants;

namespace UMS.Core.Interfaces
{
    /// <summary>
    /// Repository interface for SuperAdmin data access
    /// </summary>
    public interface ISuperAdminRepository
    {
        /// <summary>
        /// Gets SuperAdmin by email address
        /// </summary>
        Task<SuperAdmin?> GetByEmailAsync(string email);

        /// <summary>
        /// Gets SuperAdmin by ID
        /// </summary>
        Task<SuperAdmin?> GetByIdAsync(Guid id);

        /// <summary>
        /// Validates SuperAdmin credentials
        /// </summary>
        Task<SuperAdmin?> ValidateCredentialsAsync(string email, string password);

        /// <summary>
        /// Updates the last login timestamp for SuperAdmin
        /// </summary>
        Task UpdateLastLoginAsync(Guid superAdminId);

        /// <summary>
        /// Creates a new SuperAdmin account
        /// </summary>
        Task<SuperAdmin> CreateAsync(SuperAdmin superAdmin);

        /// <summary>
        /// Updates SuperAdmin profile information
        /// </summary>
        Task UpdateAsync(SuperAdmin superAdmin);

        /// <summary>
        /// Changes SuperAdmin password
        /// </summary>
        Task<bool> ChangePasswordAsync(Guid superAdminId, string currentPassword, string newPassword);
    }
}
