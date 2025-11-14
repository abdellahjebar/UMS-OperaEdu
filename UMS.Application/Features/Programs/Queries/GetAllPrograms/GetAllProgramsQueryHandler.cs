using MediatR;
using UMS.Application.DTOs.Programs;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Programs.Queries.GetAllPrograms
{
    public class GetAllProgramsQueryHandler : IRequestHandler<GetAllProgramsQuery, List<ProgramDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProgramsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProgramDto>> Handle(GetAllProgramsQuery request, CancellationToken cancellationToken)
        {
            var programs = await _unitOfWork.Programs.GetAllAsync();

            return programs.Select(p => new ProgramDto
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Code = p.Code ?? string.Empty,
                DegreeType = p.DegreeType,
                DepartmentId = p.DepartmentId,
                RequiredCredits = p.RequiredCredits,
                DurationYears = p.DurationYears,
                Description = p.Description
            }).ToList();
        }
    }
}
