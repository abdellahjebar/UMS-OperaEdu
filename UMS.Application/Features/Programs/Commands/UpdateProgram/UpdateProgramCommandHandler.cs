using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Programs.Commands.UpdateProgram
{
    public class UpdateProgramCommandHandler : IRequestHandler<UpdateProgramCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProgramCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await _unitOfWork.Programs.GetByIdAsync(request.Id);
            
            if (program == null)
            {
                throw new BadRequestException($"Program with ID {request.Id} not found");
            }

            program.Name = request.Name;
            program.Code = request.Code;
            program.DegreeType = request.DegreeType;
            program.DepartmentId = request.DepartmentId;
            program.RequiredCredits = request.RequiredCredits;
            program.DurationYears = request.DurationYears;
            program.Description = request.Description;

            await _unitOfWork.Programs.UpdateAsync(program);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
