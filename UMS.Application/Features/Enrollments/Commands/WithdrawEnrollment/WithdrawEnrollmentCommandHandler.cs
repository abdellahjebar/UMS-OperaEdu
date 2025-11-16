using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Enums;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Enrollments.Commands.WithdrawEnrollment
{
    public class WithdrawEnrollmentCommandHandler : IRequestHandler<WithdrawEnrollmentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public WithdrawEnrollmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(WithdrawEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.Id);
            
            if (enrollment == null)
            {
                throw new NotFoundException($"Enrollment with ID {request.Id} not found");
            }

            // Only decrement if currently enrolled (not already withdrawn)
            if (enrollment.Status == EnrollmentStatus.Enrolled)
            {
                // Get the section to decrement enrollment count
                var section = await _unitOfWork.Sections.GetByIdAsync(enrollment.SectionId);
                if (section != null && section.CurrentEnrollment > 0)
                {
                    section.CurrentEnrollment--;
                    await _unitOfWork.Sections.UpdateAsync(section);
                }
            }

            enrollment.Status = EnrollmentStatus.Withdrawn;

            await _unitOfWork.Enrollments.UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
