using MediatR;
using System;
using System.Security.Cryptography;
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
            // Validate that program exists
            var program = await _unitOfWork.Programs.GetByIdAsync(request.ProgramId);
            if (program == null)
            {
                throw new InvalidOperationException($"Program with ID '{request.ProgramId}' does not exist.");
            }

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

            var passwordToHash = string.IsNullOrWhiteSpace(request.Password)
                ? GenerateSecurePassword()
                : request.Password!;

            // Hash password (in production, use proper password hashing like BCrypt)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordToHash);

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
                AcademicStatus = request.AcademicStatus,
                GPA = request.GPA,
                TotalCredits = request.TotalCredits
            };

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return student.Id;
        }

        private static string GenerateSecurePassword(int length = 12)
        {
            const string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?";
            var charArray = new char[length];
            var randomBytes = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            for (var i = 0; i < length; i++)
            {
                var idx = randomBytes[i] % allowedChars.Length;
                charArray[i] = allowedChars[idx];
            }

            return new string(charArray);
        }
    }
}
