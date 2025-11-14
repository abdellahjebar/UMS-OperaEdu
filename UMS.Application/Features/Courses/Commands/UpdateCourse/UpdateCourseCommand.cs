using MediatR;

namespace UMS.Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
