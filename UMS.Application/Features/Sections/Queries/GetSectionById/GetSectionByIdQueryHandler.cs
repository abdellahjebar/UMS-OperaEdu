using MediatR;
using UMS.Application.DTOs.Sections;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Sections.Queries.GetSectionById
{
    public class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, SectionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSectionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SectionDto> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
        {
            var section = await _unitOfWork.Sections.GetByIdAsync(request.Id);
            if (section == null)
            {
                throw new NotFoundException($"Section with ID {request.Id} not found");
            }

            return new SectionDto
            {
                Id = section.Id,
                CourseId = section.CourseId,
                SectionNumber = section.SectionNumber,
                Term = section.Term,
                Year = section.Year,
                InstructorId = section.InstructorId,
                MaxCapacity = section.MaxCapacity,
                CurrentEnrollment = section.CurrentEnrollment,
                StartDate = section.StartDate,
                EndDate = section.EndDate,
                CreatedAt = section.CreatedAt,
                UpdatedAt = section.UpdatedAt
            };
        }
    }
}
