using MediatR;
using UMS.Application.DTOs.Faculty;

namespace UMS.Application.Features.Faculty.Queries.GetFacultyByDepartment
{
    public class GetFacultyByDepartmentQuery : IRequest<List<FacultyDto>>
    {
        public Guid DepartmentId { get; set; }
    }
}
