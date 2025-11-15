using Microsoft.EntityFrameworkCore;
using UMS.Core.Entities.Academic;
using UMS.Core.Enums;
using UMS.Core.Interfaces.Repositories;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistence.Repositories
{
    public class SectionRepository : Repository<Section>, ISectionRepository
    {
        public SectionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Section>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Sections
                .Where(s => s.CourseId == courseId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Section>> GetByInstructorIdAsync(Guid instructorId)
        {
            return await _context.Sections
                .Where(s => s.InstructorId == instructorId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Section>> GetByTermAndYearAsync(Term term, int year)
        {
            return await _context.Sections
                .Where(s => s.Term == term && s.Year == year && !s.IsDeleted)
                .ToListAsync();
        }
    }
}
