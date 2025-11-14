using MediatR;
using UMS.Application.DTOs.Courses;

namespace UMS.Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<List<CourseDto>>
    {
    }
}
