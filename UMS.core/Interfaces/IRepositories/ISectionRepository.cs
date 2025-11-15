using UMS.Core.Entities.Academic;
using UMS.Core.Enums;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Core.Interfaces.Repositories
{
    public interface ISectionRepository : IRepository<Section>
    {
        Task<List<Section>> GetByCourseIdAsync(Guid courseId);
        Task<List<Section>> GetByInstructorIdAsync(Guid instructorId);
        Task<List<Section>> GetByTermAndYearAsync(Term term, int year);
    }
}
