using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Enrollments.Commands.UpdateEnrollment
{
    public class UpdateEnrollmentGradeCommandHandler : IRequestHandler<UpdateEnrollmentGradeCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEnrollmentGradeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateEnrollmentGradeCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.Id);
            
            if (enrollment == null)
            {
                throw new NotFoundException($"Enrollment with ID {request.Id} not found");
            }

            enrollment.NumericGrade = request.NumericGrade;
            enrollment.GradePoints = request.GradePoints;

            await _unitOfWork.Enrollments.UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
