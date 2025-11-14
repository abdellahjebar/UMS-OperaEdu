using MediatR;
using System.Collections.Generic;
using UMS.Application.DTOs.Students;

namespace UMS.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsQuery : IRequest<IEnumerable<StudentDto>>
    {
    }
}
