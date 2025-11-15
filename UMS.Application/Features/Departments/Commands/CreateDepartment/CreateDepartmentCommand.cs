using MediatR;

namespace UMS.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? HeadOfDepartmentId { get; set; }
        public string? Building { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
