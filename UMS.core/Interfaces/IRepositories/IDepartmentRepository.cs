using UMS.Core.Entities.Academic;

namespace UMS.Core.Interfaces.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department?> GetByCodeAsync(string code);
        Task<IEnumerable<Department>> GetByBuildingAsync(string building);
    }
}
