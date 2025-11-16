using MediatR;
using UMS.Application.DTOs.Enrollments;

namespace UMS.Application.Features.Enrollments.Queries.GetSectionEnrollments
{
    public class GetSectionEnrollmentsQuery : IRequest<List<EnrollmentDto>>
    {
        public Guid SectionId { get; set; }
    }
}
