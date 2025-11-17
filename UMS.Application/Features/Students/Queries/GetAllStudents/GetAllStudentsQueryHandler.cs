using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UMS.Application.DTOs.Common;
using UMS.Application.DTOs.Students;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, PagedResult<StudentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllStudentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.Students.GetAllWithProgramAsync();
            var query = students.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(s => 
                    (s.FirstName != null && s.FirstName.ToLower().Contains(searchTerm)) ||
                    (s.LastName != null && s.LastName.ToLower().Contains(searchTerm)) ||
                    (s.Email != null && s.Email.ToLower().Contains(searchTerm)) ||
                    (s.StudentNumber != null && s.StudentNumber.ToLower().Contains(searchTerm)));
            }

            // Apply program filter
            if (request.ProgramId.HasValue)
            {
                query = query.Where(s => s.ProgramId == request.ProgramId.Value);
            }

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "firstname" => request.SortDescending ? query.OrderByDescending(s => s.FirstName) : query.OrderBy(s => s.FirstName),
                "lastname" => request.SortDescending ? query.OrderByDescending(s => s.LastName) : query.OrderBy(s => s.LastName),
                "email" => request.SortDescending ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
                "studentnumber" => request.SortDescending ? query.OrderByDescending(s => s.StudentNumber) : query.OrderBy(s => s.StudentNumber),
                "enrollmentdate" => request.SortDescending ? query.OrderByDescending(s => s.EnrollmentDate) : query.OrderBy(s => s.EnrollmentDate),
                "gpa" => request.SortDescending ? query.OrderByDescending(s => s.GPA) : query.OrderBy(s => s.GPA),
                _ => request.SortDescending ? query.OrderByDescending(s => s.LastName) : query.OrderBy(s => s.LastName)
            };

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply pagination
            var pagedStudents = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(student => new StudentDto
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
                })
                .ToList();

            return new PagedResult<StudentDto>(pagedStudents, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
