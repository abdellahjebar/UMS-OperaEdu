using MediatR;
using UMS.Application.DTOs.Faculty;

namespace UMS.Application.Features.Faculty.Queries.GetAllFaculty
{
    public class GetAllFacultyQuery : IRequest<List<FacultyDto>>
    {
    }
}
