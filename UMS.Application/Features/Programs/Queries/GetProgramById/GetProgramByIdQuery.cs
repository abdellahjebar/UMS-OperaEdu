using MediatR;
using UMS.Application.DTOs.Programs;

namespace UMS.Application.Features.Programs.Queries.GetProgramById
{
    public class GetProgramByIdQuery : IRequest<ProgramDto>
    {
        public Guid Id { get; set; }
    }
}
