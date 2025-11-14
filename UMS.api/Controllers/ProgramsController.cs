using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.Features.Programs.Commands.CreateProgram;
using UMS.Application.Features.Programs.Commands.DeleteProgram;
using UMS.Application.Features.Programs.Commands.UpdateProgram;
using UMS.Application.Features.Programs.Queries.GetAllPrograms;
using UMS.Application.Features.Programs.Queries.GetProgramById;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProgramsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProgramsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var programs = await _mediator.Send(new GetAllProgramsQuery());
            return Ok(programs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var program = await _mediator.Send(new GetProgramByIdQuery { Id = id });
            return Ok(program);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProgramCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProgramCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProgramCommand { Id = id });
            return NoContent();
        }
    }
}
