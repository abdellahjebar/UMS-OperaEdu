using UMS.Core.Entities.Identity;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Core.Interfaces.Repositories
{
    public interface IFacultyRepository : IRepository<Faculty>
    {
        Task<List<Faculty>> GetByDepartmentIdAsync(Guid departmentId);
        Task<Faculty?> GetByEmailAsync(string email);
        Task<Faculty?> GetByEmployeeNumberAsync(string employeeNumber);
    }
}
