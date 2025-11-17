using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMS.Core.Entities.Identity;
using UMS.Core.Interfaces.Repositories;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Student?> GetByStudentNumberAsync(string studentNumber)
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<IEnumerable<Student>> GetByProgramAsync(Guid programId)
        {
            return await _dbSet
                .Where(s => !s.IsDeleted && s.ProgramId == programId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string studentNumber)
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .AnyAsync(s => s.StudentNumber == studentNumber);
        }

        public async Task<IEnumerable<Student>> GetAllWithProgramAsync()
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .Include(s => s.Program)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdWithProgramAsync(Guid id)
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .Include(s => s.Program)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
