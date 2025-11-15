using MediatR;
using UMS.Application.DTOs.Enrollments;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Enrollments.Queries.GetEnrollmentById
{
    public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEnrollmentByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EnrollmentDto> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.Id);

            if (enrollment == null)
            {
                throw new NotFoundException($"Enrollment with ID {request.Id} not found");
            }

            return new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                SectionId = enrollment.SectionId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                NumericGrade = enrollment.NumericGrade,
                GradePoints = enrollment.GradePoints
            };
        }
    }
}
