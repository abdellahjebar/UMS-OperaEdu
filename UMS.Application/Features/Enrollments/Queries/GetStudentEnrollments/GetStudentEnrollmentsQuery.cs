using MediatR;
using UMS.Application.DTOs.Enrollments;

namespace UMS.Application.Features.Enrollments.Queries.GetStudentEnrollments
{
    public class GetStudentEnrollmentsQuery : IRequest<List<EnrollmentDto>>
    {
        public Guid StudentId { get; set; }
    }
}
