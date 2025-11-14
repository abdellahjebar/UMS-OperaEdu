using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Core.Entities.Academic;

namespace UMS.Core.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetByCodeAsync(string code);
        Task<IEnumerable<Course>> GetByDepartmentAsync(Guid departmentId);
        Task<bool> ExistsAsync(string code);
    }
}