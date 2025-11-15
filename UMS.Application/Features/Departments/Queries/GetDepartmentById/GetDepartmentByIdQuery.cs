using MediatR;
using UMS.Application.DTOs.Departments;

namespace UMS.Application.Features.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQuery : IRequest<DepartmentDto>
    {
        public Guid Id { get; set; }
    }
}
