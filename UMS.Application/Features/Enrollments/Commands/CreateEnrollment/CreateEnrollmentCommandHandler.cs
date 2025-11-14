using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Enums;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Enrollments.Commands.CreateEnrollment
{
    public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEnrollmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                SectionId = request.SectionId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Enrolled
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return enrollment.Id;
        }
    }
}
