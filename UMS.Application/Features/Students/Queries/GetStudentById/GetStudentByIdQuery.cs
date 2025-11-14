using MediatR;
using System;
using UMS.Application.DTOs.Students;

namespace UMS.Application.Features.Students.Queries.GetStudentById
{
    public class GetStudentByIdQuery : IRequest<StudentDto>
    {
        public Guid Id { get; set; }
    }
}
