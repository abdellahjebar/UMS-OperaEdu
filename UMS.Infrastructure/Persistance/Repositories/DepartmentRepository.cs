using Microsoft.EntityFrameworkCore;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Department?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .Where(d => !d.IsDeleted)
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<IEnumerable<Department>> GetByBuildingAsync(string building)
        {
            return await _dbSet
                .Where(d => !d.IsDeleted && d.Building == building)
                .ToListAsync();
        }
    }
}
