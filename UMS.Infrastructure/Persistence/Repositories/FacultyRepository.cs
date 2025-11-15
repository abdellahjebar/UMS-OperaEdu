using Microsoft.EntityFrameworkCore;
using UMS.Core.Entities.Identity;
using UMS.Core.Interfaces.Repositories;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistence.Repositories
{
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        public FacultyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Faculty>> GetByDepartmentIdAsync(Guid departmentId)
        {
            return await _context.Set<Faculty>()
                .Where(f => f.DepartmentId == departmentId && !f.IsDeleted)
                .ToListAsync();
        }

        public async Task<Faculty?> GetByEmailAsync(string email)
        {
            return await _context.Set<Faculty>()
                .FirstOrDefaultAsync(f => f.Email == email && !f.IsDeleted);
        }

        public async Task<Faculty?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            return await _context.Set<Faculty>()
                .FirstOrDefaultAsync(f => f.EmployeeNumber == employeeNumber && !f.IsDeleted);
        }
    }
}
