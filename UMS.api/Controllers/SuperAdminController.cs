using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UMS.Application.Features.SuperAdmin.Commands.Login;

namespace UMS.API.Controllers
{
    /// <summary>
    /// Controller for SuperAdmin authentication and management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SuperAdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SuperAdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// SuperAdmin login endpoint
        /// </summary>
        /// <param name="command">Login credentials</param>
        /// <returns>JWT token and SuperAdmin information</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(SuperAdminLoginResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] SuperAdminLoginCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
