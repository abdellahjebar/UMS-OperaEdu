using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Application.DTOs.Students;
using UMS.Application.Features.Students.Commands.CreateStudent;
using UMS.Application.Features.Students.Queries.GetAllStudents;
using UMS.Application.Features.Students.Queries.GetStudentById;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all students (Admin/Faculty only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Faculty")]
        [ProducesResponseType(typeof(IEnumerable<StudentDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var students = await _mediator.Send(new GetAllStudentsQuery());
            return Ok(students);
        }

        /// <summary>
        /// Get student by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var student = await _mediator.Send(new GetStudentByIdQuery { Id = id });
                return Ok(student);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new student (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
        {
            try
            {
                var studentId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = studentId }, new { id = studentId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
