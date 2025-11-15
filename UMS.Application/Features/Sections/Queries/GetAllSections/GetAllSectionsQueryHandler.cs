using MediatR;
using UMS.Application.DTOs.Sections;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Sections.Queries.GetAllSections
{
    public class GetAllSectionsQueryHandler : IRequestHandler<GetAllSectionsQuery, List<SectionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSectionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SectionDto>> Handle(GetAllSectionsQuery request, CancellationToken cancellationToken)
        {
            var sections = await _unitOfWork.Sections.GetAllAsync();

            return sections.Select(s => new SectionDto
            {
                Id = s.Id,
                CourseId = s.CourseId,
                SectionNumber = s.SectionNumber,
                Term = s.Term,
                Year = s.Year,
                InstructorId = s.InstructorId,
                MaxCapacity = s.MaxCapacity,
                CurrentEnrollment = s.CurrentEnrollment,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }
    }
}
