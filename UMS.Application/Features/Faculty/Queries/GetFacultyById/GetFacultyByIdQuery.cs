using MediatR;
using UMS.Application.DTOs.Faculty;

namespace UMS.Application.Features.Faculty.Queries.GetFacultyById
{
    public class GetFacultyByIdQuery : IRequest<FacultyDto>
    {
        public Guid Id { get; set; }
    }
}
