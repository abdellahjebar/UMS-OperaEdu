using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistence.Repositories
{
    public class ProgramRepository : Repository<Program>, IProgramRepository
    {
        public ProgramRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Program?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted)
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<IEnumerable<Program>> GetByDepartmentAsync(Guid departmentId)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted && p.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted)
                .AnyAsync(p => p.Code == code);
        }
    }
}
