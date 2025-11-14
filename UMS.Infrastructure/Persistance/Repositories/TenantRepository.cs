using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UMS.Core.Entities.Tenants;
using UMS.Core.Interfaces.IRepositories;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistence.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly MasterDbContext _context;

        public TenantRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetByIdAsync(Guid id)
        {
            return await _context.Tenants
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant?> GetBySubdomainAsync(string subdomain)
        {
            return await _context.Tenants
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.Subdomain.ToLower() == subdomain.ToLower());
        }

        public async Task<System.Collections.Generic.IEnumerable<Tenant>> GetAllAsync()
        {
            return await _context.Tenants
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<System.Collections.Generic.IEnumerable<Tenant>> FindAsync(System.Linq.Expressions.Expression<Func<Tenant, bool>> predicate)
        {
            return await _context.Tenants
                .Where(t => !t.IsDeleted)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<bool> SubdomainExistsAsync(string subdomain)
        {
            return await _context.Tenants
                .Where(t => !t.IsDeleted)
                .AnyAsync(t => t.Subdomain.ToLower() == subdomain.ToLower());
        }

        public async Task<bool> IsConnectionStringValidAsync(string connectionString)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using var context = new ApplicationDbContext(optionsBuilder.Options);
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task AddAsync(Tenant entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Tenants.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tenant entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Tenants.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tenant = await GetByIdAsync(id);
            if (tenant != null)
            {
                tenant.IsDeleted = true;
                tenant.DeletedAt = DateTime.UtcNow;
                _context.Tenants.Update(tenant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
