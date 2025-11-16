using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Sections;
using UMS.Application.Features.Sections.Commands.CreateSection;
using UMS.Application.Features.Sections.Commands.DeleteSection;
using UMS.Application.Features.Sections.Commands.UpdateSection;
using UMS.Application.Features.Sections.Queries.GetAllSections;
using UMS.Application.Features.Sections.Queries.GetSectionById;
using UMS.Application.Features.Enrollments.Queries.GetSectionEnrollments;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "FacultyOrAdmin")]
        public async Task<ActionResult<List<SectionDto>>> GetAll()
        {
            var sections = await _mediator.Send(new GetAllSectionsQuery());
            return Ok(sections);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "FacultyOrAdmin")]
        public async Task<ActionResult<SectionDto>> GetById(Guid id)
        {
            var section = await _mediator.Send(new GetSectionByIdQuery { Id = id });
            return Ok(section);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateSectionCommand command)
        {
            var sectionId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = sectionId }, sectionId);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateSectionCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteSectionCommand { Id = id });
            return NoContent();
        }

        [HttpGet("{id}/enrollments")]
        [Authorize(Policy = "FacultyOrAdmin")]
        public async Task<ActionResult> GetEnrollments(Guid id)
        {
            var enrollments = await _mediator.Send(new GetSectionEnrollmentsQuery { SectionId = id });
            return Ok(enrollments);
        }
    }
}
