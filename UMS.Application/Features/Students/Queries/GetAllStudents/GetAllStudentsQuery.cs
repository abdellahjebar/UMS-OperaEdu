using MediatR;
using System;
using UMS.Application.DTOs.Common;
using UMS.Application.DTOs.Students;

namespace UMS.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsQuery : IRequest<PagedResult<StudentDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "LastName"; // Default sort by last name
        public bool SortDescending { get; set; } = false;
        public Guid? ProgramId { get; set; } // Filter by program
    }
}
