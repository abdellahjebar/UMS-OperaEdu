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
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted && e.StudentId == studentId)
                .Include(e => e.Section)
                .ThenInclude(s => s.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetBySectionIdAsync(Guid sectionId)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted && e.SectionId == sectionId)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByStudentAndSectionAsync(Guid studentId, Guid sectionId)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SectionId == sectionId);
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid sectionId)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .AnyAsync(e => e.StudentId == studentId && e.SectionId == sectionId);
        }

        public async Task<int> GetStudentEnrollmentCountAsync(Guid studentId)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted && e.StudentId == studentId)
                .CountAsync();
        }
    }
}
