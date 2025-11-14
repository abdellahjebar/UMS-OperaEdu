using MediatR;
using UMS.Application.DTOs.Courses;

namespace UMS.Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQuery : IRequest<CourseDto>
    {
        public Guid Id { get; set; }
    }
}
