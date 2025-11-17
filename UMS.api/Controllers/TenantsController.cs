using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Application.DTOs.Tenants;
using UMS.Application.Features.Tenants.Commands.CreateTenant;
using UMS.Application.Features.Tenants.Queries.GetAllTenants;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "SuperAdminOnly")]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITenantService _tenantService;

        public TenantsController(IMediator mediator, ITenantService tenantService)
        {
            _mediator = mediator;
            _tenantService = tenantService;
        }

        /// <summary>
        /// Get all tenants (Super Admin only)
        /// Access via: admin.yourdomain.com/api/tenants
        /// Requires JWT token with SuperAdmin role
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TenantDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await _mediator.Send(new GetAllTenantsQuery());
            return Ok(tenants);
        }

        /// <summary>
        /// Create a new tenant/school (Super Admin only)
        /// Access via: admin.yourdomain.com/api/tenants
        /// This will create a new database for the school
        /// Requires JWT token with SuperAdmin role
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create([FromBody] CreateTenantCommand command)
        {
            // InvalidOperationException thrown by handler is automatically caught by ExceptionHandlingMiddleware
            var tenantId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = tenantId }, new { id = tenantId, subdomain = command.Subdomain });
        }
    }
}
