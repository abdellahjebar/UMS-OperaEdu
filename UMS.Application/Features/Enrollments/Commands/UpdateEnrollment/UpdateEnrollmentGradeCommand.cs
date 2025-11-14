using MediatR;

namespace UMS.Application.Features.Enrollments.Commands.UpdateEnrollment
{
    public class UpdateEnrollmentGradeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public decimal NumericGrade { get; set; }
        public decimal GradePoints { get; set; }
    }
}
