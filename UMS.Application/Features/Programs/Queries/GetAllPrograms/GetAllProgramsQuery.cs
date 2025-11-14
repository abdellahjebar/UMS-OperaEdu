using MediatR;
using UMS.Application.DTOs.Programs;

namespace UMS.Application.Features.Programs.Queries.GetAllPrograms
{
    public class GetAllProgramsQuery : IRequest<List<ProgramDto>>
    {
    }
}
