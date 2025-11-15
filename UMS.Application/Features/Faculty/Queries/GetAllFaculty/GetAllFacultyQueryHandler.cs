using MediatR;
using UMS.Application.DTOs.Faculty;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Faculty.Queries.GetAllFaculty
{
    public class GetAllFacultyQueryHandler : IRequestHandler<GetAllFacultyQuery, List<FacultyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllFacultyQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FacultyDto>> Handle(GetAllFacultyQuery request, CancellationToken cancellationToken)
        {
            var facultyList = await _unitOfWork.Faculty.GetAllAsync();

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
