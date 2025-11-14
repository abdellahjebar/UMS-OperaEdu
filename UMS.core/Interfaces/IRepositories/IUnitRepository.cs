using System;
using System.Threading.Tasks;

namespace UMS.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }
        ICourseRepository Courses { get; }
        IEnrollmentRepository Enrollments { get; }
        IProgramRepository Programs { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}