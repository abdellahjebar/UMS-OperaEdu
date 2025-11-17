using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UMS.Application.DTOs.Students;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Students.Queries.GetStudentById
{
    public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Students.GetByIdWithProgramAsync(request.Id);

            if (student == null)
            {
                throw new System.Exception($"Student with ID '{request.Id}' not found.");
            }

            return new StudentDto
            {
                Id = student.Id,
                Email = student.Email ?? string.Empty,
                FirstName = student.FirstName ?? string.Empty,
                LastName = student.LastName ?? string.Empty,
                PhoneNumber = student.PhoneNumber,
                DateOfBirth = student.DateOfBirth,
                StudentNumber = student.StudentNumber,
                EnrollmentDate = student.EnrollmentDate,
                ExpectedGraduationDate = student.ExpectedGraduationDate,
                ProgramId = student.ProgramId,
                ProgramName = student.Program != null ? student.Program.Name : null,
                AcademicStatus = student.AcademicStatus,
                GPA = student.GPA,
                TotalCredits = student.TotalCredits,
                IsActive = student.IsActive
            };
        }
    }
}
