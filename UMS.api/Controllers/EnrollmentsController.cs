using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.Features.Enrollments.Commands.CreateEnrollment;
using UMS.Application.Features.Enrollments.Commands.UpdateEnrollment;
using UMS.Application.Features.Enrollments.Commands.WithdrawEnrollment;
using UMS.Application.Features.Enrollments.Queries.GetEnrollmentById;
using UMS.Application.Features.Enrollments.Queries.GetStudentEnrollments;
using UMS.Core.Interfaces.Repositories;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentsController(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _unitOfWork.Enrollments.GetAllAsync();
            return Ok(enrollments);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(Guid studentId)
        {
            var enrollments = await _mediator.Send(new GetStudentEnrollmentsQuery { StudentId = studentId });
            return Ok(enrollments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var enrollment = await _mediator.Send(new GetEnrollmentByIdQuery { Id = id });
            return Ok(enrollment);
        }

        [HttpPost]
        [Authorize(Policy = "StudentOnly")]
        public async Task<IActionResult> Create([FromBody] CreateEnrollmentCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}/grade")]
        [Authorize(Policy = "FacultyOrAdmin")]
        public async Task<IActionResult> UpdateGrade(Guid id, [FromBody] UpdateEnrollmentGradeCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{id}/withdraw")]
        [Authorize(Policy = "StudentOrFacultyOrAdmin")]
        public async Task<IActionResult> Withdraw(Guid id)
        {
            await _mediator.Send(new WithdrawEnrollmentCommand { Id = id });
            return NoContent();
        }
    }
}
