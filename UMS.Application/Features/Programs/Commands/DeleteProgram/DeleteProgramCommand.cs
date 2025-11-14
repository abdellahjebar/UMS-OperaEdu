using MediatR;

namespace UMS.Application.Features.Programs.Commands.DeleteProgram
{
    public class DeleteProgramCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
