using MediatR;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(request.Id);
            if (department == null)
                throw new NotFoundException($"Department with ID {request.Id} not found");

            department.Name = request.Name;
            department.Code = request.Code;
            department.Description = request.Description;
            department.HeadOfDepartmentId = request.HeadOfDepartmentId;
            department.Building = request.Building;
            department.Phone = request.Phone;
            department.Email = request.Email;
            department.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Departments.UpdateAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
