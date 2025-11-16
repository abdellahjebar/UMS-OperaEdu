using MediatR;
using UMS.Application.DTOs.Enrollments;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Enrollments.Queries.GetSectionEnrollments
{
    public class GetSectionEnrollmentsQueryHandler : IRequestHandler<GetSectionEnrollmentsQuery, List<EnrollmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSectionEnrollmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EnrollmentDto>> Handle(GetSectionEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _unitOfWork.Enrollments.GetBySectionIdAsync(request.SectionId);

            return enrollments.Select(e => new EnrollmentDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                SectionId = e.SectionId,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                NumericGrade = e.NumericGrade,
                GradePoints = e.GradePoints
            }).ToList();
        }
    }
}
