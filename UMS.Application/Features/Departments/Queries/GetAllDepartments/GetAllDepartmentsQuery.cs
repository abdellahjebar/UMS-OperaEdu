using MediatR;
using UMS.Application.DTOs.Departments;

namespace UMS.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>
    {
    }
}
