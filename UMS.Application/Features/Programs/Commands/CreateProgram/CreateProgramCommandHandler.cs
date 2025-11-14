using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Programs.Commands.CreateProgram
{
    public class CreateProgramCommandHandler : IRequestHandler<CreateProgramCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProgramCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
        {
            var program = new Program
            {
                Name = request.Name,
                Code = request.Code,
                DegreeType = request.DegreeType,
                DepartmentId = request.DepartmentId,
                RequiredCredits = request.RequiredCredits,
                DurationYears = request.DurationYears,
                Description = request.Description
            };

            await _unitOfWork.Programs.AddAsync(program);
            await _unitOfWork.SaveChangesAsync();

            return program.Id;
        }
    }
}
