using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Enums;
using UMS.Core.Exceptions;
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
            // Check if student is already enrolled in this section
            var isAlreadyEnrolled = await _unitOfWork.Enrollments.IsStudentEnrolledAsync(request.StudentId, request.SectionId);
            if (isAlreadyEnrolled)
            {
                throw new InvalidOperationException($"Student is already enrolled in this section");
            }

            // Get the section to check capacity
            var section = await _unitOfWork.Sections.GetByIdAsync(request.SectionId);
            if (section == null)
            {
                throw new NotFoundException($"Section with ID {request.SectionId} not found");
            }

            // Check if section has available capacity
            if (section.CurrentEnrollment >= section.MaxCapacity)
            {
                throw new InvalidOperationException($"Section is full (capacity: {section.MaxCapacity})");
            }

            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                SectionId = request.SectionId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Enrolled
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);

            // Increment the section's current enrollment count
            section.CurrentEnrollment++;
            await _unitOfWork.Sections.UpdateAsync(section);

            await _unitOfWork.SaveChangesAsync();

            return enrollment.Id;
        }
    }
}
