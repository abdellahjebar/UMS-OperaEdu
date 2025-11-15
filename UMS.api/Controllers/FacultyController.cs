using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Faculty;
using UMS.Application.Features.Faculty.Commands.CreateFaculty;
using UMS.Application.Features.Faculty.Commands.DeleteFaculty;
using UMS.Application.Features.Faculty.Commands.UpdateFaculty;
using UMS.Application.Features.Faculty.Queries.GetAllFaculty;
using UMS.Application.Features.Faculty.Queries.GetFacultyByDepartment;
using UMS.Application.Features.Faculty.Queries.GetFacultyById;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FacultyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all faculty members
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<FacultyDto>>> GetAllFaculty()
        {
            var query = new GetAllFacultyQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get faculty member by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FacultyDto>> GetFacultyById(Guid id)
        {
            var query = new GetFacultyByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get faculty members by department
        /// </summary>
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<FacultyDto>>> GetFacultyByDepartment(Guid departmentId)
        {
            var query = new GetFacultyByDepartmentQuery { DepartmentId = departmentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Create a new faculty member (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Guid>> CreateFaculty([FromBody] CreateFacultyCommand command)
        {
            var facultyId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetFacultyById), new { id = facultyId }, new { id = facultyId });
        }

        /// <summary>
        /// Update faculty member (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> UpdateFaculty(Guid id, [FromBody] UpdateFacultyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Delete faculty member (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteFaculty(Guid id)
        {
            var command = new DeleteFacultyCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
