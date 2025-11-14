using MediatR;

namespace UMS.Application.Features.Enrollments.Commands.WithdrawEnrollment
{
    public class WithdrawEnrollmentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
