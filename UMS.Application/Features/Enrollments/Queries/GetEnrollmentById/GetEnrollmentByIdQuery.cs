using MediatR;
using UMS.Application.DTOs.Enrollments;

namespace UMS.Application.Features.Enrollments.Queries.GetEnrollmentById
{
    public class GetEnrollmentByIdQuery : IRequest<EnrollmentDto>
    {
        public Guid Id { get; set; }
    }
}
