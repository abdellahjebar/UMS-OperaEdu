using MediatR;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Faculty.Commands.UpdateFaculty
{
    public class UpdateFacultyCommandHandler : IRequestHandler<UpdateFacultyCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateFacultyCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _unitOfWork.Faculty.GetByIdAsync(request.Id);
            if (faculty == null)
            {
                throw new NotFoundException($"Faculty with ID {request.Id} not found");
            }

            faculty.Email = request.Email;
            faculty.FirstName = request.FirstName;
            faculty.LastName = request.LastName;
            faculty.PhoneNumber = request.PhoneNumber;
            faculty.EmployeeNumber = request.EmployeeNumber;
            faculty.DepartmentId = request.DepartmentId;
            faculty.Title = request.Title;
            faculty.HireDate = request.HireDate;
            faculty.OfficeLocation = request.OfficeLocation;
            faculty.OfficeHours = request.OfficeHours;
            faculty.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Faculty.UpdateAsync(faculty);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
