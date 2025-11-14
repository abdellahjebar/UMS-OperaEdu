using MediatR;
using Microsoft.EntityFrameworkCore;
using UMS.Application.DTOs.Enrollments;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Enrollments.Queries.GetStudentEnrollments
{
    public class GetStudentEnrollmentsQueryHandler : IRequestHandler<GetStudentEnrollmentsQuery, List<EnrollmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentEnrollmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EnrollmentDto>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _unitOfWork.Enrollments.GetByStudentIdAsync(request.StudentId);

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
