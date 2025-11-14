using UMS.Core.Entities.Identity;
using UMS.Core.Interfaces.IRepositories;

namespace UMS.Core.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }
}
