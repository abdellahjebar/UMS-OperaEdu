using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Core.Entities.Identity;

namespace UMS.Core.Interfaces.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetByStudentNumberAsync(string studentNumber);
        Task<Student?> GetByEmailAsync(string email);
        Task<IEnumerable<Student>> GetByProgramAsync(Guid programId);
        Task<bool> ExistsAsync(string studentNumber);
    }
}