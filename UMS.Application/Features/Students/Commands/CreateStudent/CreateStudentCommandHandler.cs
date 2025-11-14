using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UMS.Core.Entities.Identity;
using UMS.Core.Enums;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // Check if student number already exists
            if (!string.IsNullOrEmpty(request.StudentNumber))
            {
                var exists = await _unitOfWork.Students.ExistsAsync(request.StudentNumber);
                if (exists)
                {
                    throw new InvalidOperationException($"Student with number '{request.StudentNumber}' already exists.");
                }
            }

            // Check if email already exists
            var emailExists = await _unitOfWork.Students.GetByEmailAsync(request.Email);
            if (emailExists != null)
            {
                throw new InvalidOperationException($"User with email '{request.Email}' already exists.");
            }

            // Hash password (in production, use proper password hashing like BCrypt)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var student = new Student
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                UserType = UserType.Student,
                IsActive = true,
                EmailConfirmed = false,
                StudentNumber = request.StudentNumber,
                EnrollmentDate = request.EnrollmentDate,
                ExpectedGraduationDate = request.ExpectedGraduationDate,
                ProgramId = request.ProgramId,
                AcademicStatus = AcademicStatus.Active,
                GPA = 0.0m,
                TotalCredits = 0
            };

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return student.Id;
        }
    }
}
