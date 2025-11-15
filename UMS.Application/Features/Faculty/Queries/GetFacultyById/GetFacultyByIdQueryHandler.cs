using MediatR;
using UMS.Application.DTOs.Faculty;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Faculty.Queries.GetFacultyById
{
    public class GetFacultyByIdQueryHandler : IRequestHandler<GetFacultyByIdQuery, FacultyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFacultyByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FacultyDto> Handle(GetFacultyByIdQuery request, CancellationToken cancellationToken)
        {
            var faculty = await _unitOfWork.Faculty.GetByIdAsync(request.Id);
            if (faculty == null)
            {
                throw new NotFoundException($"Faculty with ID {request.Id} not found");
            }

            return new FacultyDto
            {
                Id = faculty.Id,
                Email = faculty.Email ?? string.Empty,
                FirstName = faculty.FirstName ?? string.Empty,
                LastName = faculty.LastName ?? string.Empty,
                PhoneNumber = faculty.PhoneNumber ?? string.Empty,
                EmployeeNumber = faculty.EmployeeNumber,
                DepartmentId = faculty.DepartmentId,
                Title = faculty.Title,
                HireDate = faculty.HireDate,
                OfficeLocation = faculty.OfficeLocation,
                OfficeHours = faculty.OfficeHours,
                CreatedAt = faculty.CreatedAt,
                UpdatedAt = faculty.UpdatedAt
            };
        }
    }
}
