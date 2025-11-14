using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Core.Entities.Academic;

namespace UMS.Core.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId);
        Task<IEnumerable<Enrollment>> GetBySectionIdAsync(Guid sectionId);
        Task<Enrollment?> GetByStudentAndSectionAsync(Guid studentId, Guid sectionId);
        Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid sectionId);
        Task<int> GetStudentEnrollmentCountAsync(Guid studentId);
    }
}