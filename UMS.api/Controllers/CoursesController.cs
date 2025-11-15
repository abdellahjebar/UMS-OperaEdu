using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.Features.Courses.Commands.CreateCourse;
using UMS.Application.Features.Courses.Commands.DeleteCourse;
using UMS.Application.Features.Courses.Commands.UpdateCourse;
using UMS.Application.Features.Courses.Queries.GetAllCourses;
using UMS.Application.Features.Courses.Queries.GetCourseById;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _mediator.Send(new GetAllCoursesQuery());
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _mediator.Send(new GetCourseByIdQuery { Id = id });
            return Ok(course);
        }

        [HttpPost]
        [Authorize(Policy = "FacultyOrAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateCourseCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "FacultyOrAdmin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "FacultyOrAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteCourseCommand { Id = id });
            return NoContent();
        }
    }
}
