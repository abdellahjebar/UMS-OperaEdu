using MediatR;

namespace UMS.Application.Features.Enrollments.Commands.CreateEnrollment
{
    public class CreateEnrollmentCommand : IRequest<Guid>
    {
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
    }
}
