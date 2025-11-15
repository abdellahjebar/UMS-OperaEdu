using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = new Department
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                HeadOfDepartmentId = request.HeadOfDepartmentId,
                Building = request.Building,
                Phone = request.Phone,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return department.Id;
        }
    }
}
