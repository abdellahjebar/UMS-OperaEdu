using MediatR;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(request.Id);
            if (department == null)
                throw new NotFoundException($"Department with ID {request.Id} not found");

            await _unitOfWork.Departments.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
