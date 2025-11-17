using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UMS.Core.Entities.Tenants;
using UMS.Core.Interfaces;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for SuperAdmin data access
    /// Operates on the Master database context
    /// </summary>
    public class SuperAdminRepository : ISuperAdminRepository
    {
        private readonly MasterDbContext _context;

        public SuperAdminRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<SuperAdmin?> GetByEmailAsync(string email)
        {
            return await _context.SuperAdmins
                .FirstOrDefaultAsync(sa => sa.Email == email && !sa.IsDeleted && sa.IsActive);
        }

        public async Task<SuperAdmin?> GetByIdAsync(Guid id)
        {
            return await _context.SuperAdmins
                .FirstOrDefaultAsync(sa => sa.Id == id && !sa.IsDeleted);
        }

        public async Task<SuperAdmin?> ValidateCredentialsAsync(string email, string password)
        {
            var superAdmin = await GetByEmailAsync(email);
            
            if (superAdmin == null || !superAdmin.IsActive || superAdmin.IsDeleted)
            {
                return null;
            }

            // Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, superAdmin.PasswordHash);
            
            return isPasswordValid ? superAdmin : null;
        }

        public async Task UpdateLastLoginAsync(Guid superAdminId)
        {
            var superAdmin = await GetByIdAsync(superAdminId);
            if (superAdmin != null)
            {
                superAdmin.LastLoginAt = DateTime.UtcNow;
                superAdmin.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SuperAdmin> CreateAsync(SuperAdmin superAdmin)
        {
            superAdmin.CreatedAt = DateTime.UtcNow;
            superAdmin.UpdatedAt = DateTime.UtcNow;
            
            _context.SuperAdmins.Add(superAdmin);
            await _context.SaveChangesAsync();
            
            return superAdmin;
        }

        public async Task UpdateAsync(SuperAdmin superAdmin)
        {
            superAdmin.UpdatedAt = DateTime.UtcNow;
            
            _context.SuperAdmins.Update(superAdmin);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangePasswordAsync(Guid superAdminId, string currentPassword, string newPassword)
        {
            var superAdmin = await GetByIdAsync(superAdminId);
            
            if (superAdmin == null)
            {
                return false;
            }

            // Verify current password
            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, superAdmin.PasswordHash);
            
            if (!isCurrentPasswordValid)
            {
                return false;
            }

            // Hash and set new password
            superAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            superAdmin.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
