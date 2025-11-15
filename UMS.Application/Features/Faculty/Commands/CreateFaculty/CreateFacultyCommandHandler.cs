using MediatR;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Faculty.Commands.CreateFaculty
{
    public class CreateFacultyCommandHandler : IRequestHandler<CreateFacultyCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFacultyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
        {
            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var faculty = new Core.Entities.Identity.Faculty
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                UserType = Core.Enums.UserType.Faculty,
                EmployeeNumber = request.EmployeeNumber,
                DepartmentId = request.DepartmentId,
                Title = request.Title,
                HireDate = request.HireDate,
                OfficeLocation = request.OfficeLocation,
                OfficeHours = request.OfficeHours,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Faculty.AddAsync(faculty);
            await _unitOfWork.SaveChangesAsync();

            return faculty.Id;
        }
    }
}
