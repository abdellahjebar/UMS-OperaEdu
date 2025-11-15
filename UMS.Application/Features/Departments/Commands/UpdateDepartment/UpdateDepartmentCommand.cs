using MediatR;

namespace UMS.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? HeadOfDepartmentId { get; set; }
        public string? Building { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
