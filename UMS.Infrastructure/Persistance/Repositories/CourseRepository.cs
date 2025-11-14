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
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Course?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Course>> GetByDepartmentAsync(Guid departmentId)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted && c.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .AnyAsync(c => c.Code == code);
        }
    }
}
