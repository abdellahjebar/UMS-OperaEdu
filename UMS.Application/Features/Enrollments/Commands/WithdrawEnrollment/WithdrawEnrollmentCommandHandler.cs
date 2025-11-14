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
                throw new BadRequestException($"Enrollment with ID {request.Id} not found");
            }

            enrollment.Status = EnrollmentStatus.Withdrawn;

            await _unitOfWork.Enrollments.UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
