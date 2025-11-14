namespace UMS.Application.DTOs.Courses
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
