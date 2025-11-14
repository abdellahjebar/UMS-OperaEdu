using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Core.Entities.Academic;

namespace UMS.Core.Interfaces.Repositories
{
    public interface IProgramRepository : IRepository<Program>
    {
        Task<Program?> GetByCodeAsync(string code);
        Task<IEnumerable<Program>> GetByDepartmentAsync(Guid departmentId);
        Task<bool> ExistsAsync(string code);
    }
}
