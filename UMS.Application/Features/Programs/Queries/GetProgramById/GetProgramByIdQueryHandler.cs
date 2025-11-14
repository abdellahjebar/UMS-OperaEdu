using MediatR;
using UMS.Application.DTOs.Programs;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Programs.Queries.GetProgramById
{
    public class GetProgramByIdQueryHandler : IRequestHandler<GetProgramByIdQuery, ProgramDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProgramByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProgramDto> Handle(GetProgramByIdQuery request, CancellationToken cancellationToken)
        {
            var program = await _unitOfWork.Programs.GetByIdAsync(request.Id);

            if (program == null)
            {
                throw new BadRequestException($"Program with ID {request.Id} not found");
            }

            return new ProgramDto
            {
                Id = program.Id,
                Name = program.Name ?? string.Empty,
                Code = program.Code ?? string.Empty,
                DegreeType = program.DegreeType,
                DepartmentId = program.DepartmentId,
                RequiredCredits = program.RequiredCredits,
                DurationYears = program.DurationYears,
                Description = program.Description
            };
        }
    }
}
