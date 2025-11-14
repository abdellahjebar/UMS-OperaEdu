using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Programs.Commands.DeleteProgram
{
    public class DeleteProgramCommandHandler : IRequestHandler<DeleteProgramCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProgramCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await _unitOfWork.Programs.GetByIdAsync(request.Id);
            
            if (program == null)
            {
                throw new BadRequestException($"Program with ID {request.Id} not found");
            }

            await _unitOfWork.Programs.DeleteAsync(program.Id);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
