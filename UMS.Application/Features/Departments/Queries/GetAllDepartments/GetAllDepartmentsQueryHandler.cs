using MediatR;
using UMS.Application.DTOs.Departments;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllDepartmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();

            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                HeadOfDepartmentId = d.HeadOfDepartmentId,
                Building = d.Building,
                Phone = d.Phone,
                Email = d.Email
            }).ToList();
        }
    }
}
