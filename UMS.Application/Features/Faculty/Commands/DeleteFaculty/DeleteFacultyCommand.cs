using MediatR;

namespace UMS.Application.Features.Faculty.Commands.DeleteFaculty
{
    public class DeleteFacultyCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
