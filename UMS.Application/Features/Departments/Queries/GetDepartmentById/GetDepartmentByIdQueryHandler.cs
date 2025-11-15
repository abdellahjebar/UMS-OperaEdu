using MediatR;
using UMS.Application.DTOs.Departments;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(request.Id);
            if (department == null)
                throw new NotFoundException($"Department with ID {request.Id} not found");

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                HeadOfDepartmentId = department.HeadOfDepartmentId,
                Building = department.Building,
                Phone = department.Phone,
                Email = department.Email
            };
        }
    }
}
