using MediatR;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Faculty.Commands.DeleteFaculty
{
    public class DeleteFacultyCommandHandler : IRequestHandler<DeleteFacultyCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFacultyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _unitOfWork.Faculty.GetByIdAsync(request.Id);
            if (faculty == null)
            {
                throw new NotFoundException($"Faculty with ID {request.Id} not found");
            }

            await _unitOfWork.Faculty.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
