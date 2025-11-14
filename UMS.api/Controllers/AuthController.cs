using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Auth;
using UMS.Application.Features.Auth.Commands.Login;
using UMS.Application.Features.Auth.Commands.Register;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var command = new LoginCommand
            {
                Email = loginDto.Email,
                Password = loginDto.Password
            };

            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Register a new user (Student, Faculty, or Staff)
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), new { id = response.UserId }, response);
        }
    }
}
