using MediatR;

namespace UMS.Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommand : IRequest<Guid>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
