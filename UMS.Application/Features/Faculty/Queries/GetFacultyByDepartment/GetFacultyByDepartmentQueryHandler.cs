using MediatR;
using UMS.Application.DTOs.Faculty;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Faculty.Queries.GetFacultyByDepartment
{
    public class GetFacultyByDepartmentQueryHandler : IRequestHandler<GetFacultyByDepartmentQuery, List<FacultyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFacultyByDepartmentQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FacultyDto>> Handle(GetFacultyByDepartmentQuery request, CancellationToken cancellationToken)
        {
            var facultyList = await _unitOfWork.Faculty.GetByDepartmentIdAsync(request.DepartmentId);

            return facultyList.Select(f => new FacultyDto
            {
                Id = f.Id,
                Email = f.Email,
                FirstName = f.FirstName,
                LastName = f.LastName,
                PhoneNumber = f.PhoneNumber,
                EmployeeNumber = f.EmployeeNumber,
                DepartmentId = f.DepartmentId,
                Title = f.Title,
                HireDate = f.HireDate,
                OfficeLocation = f.OfficeLocation,
                OfficeHours = f.OfficeHours,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList();
        }
    }
}
