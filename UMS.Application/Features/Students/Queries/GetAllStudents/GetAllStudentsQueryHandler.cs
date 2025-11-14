using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UMS.Application.DTOs.Students;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, IEnumerable<StudentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllStudentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.Students.GetAllAsync();

            return students.Select(student => new StudentDto
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
                AcademicStatus = student.AcademicStatus,
                GPA = student.GPA,
                TotalCredits = student.TotalCredits,
                IsActive = student.IsActive
            }).ToList();
        }
    }
}
