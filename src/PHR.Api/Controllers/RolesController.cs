using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PHR.Api.Services;
using System;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace PHR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _svc;
        public RolesController(IRoleService svc) { _svc = svc; }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create role", Tags = new[] { "Roles" })]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var r = await _svc.CreateRoleAsync(name);
            return Ok(r);
        }

        [HttpPost("{roleId}/permissions/{permId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Assign permission to role", Tags = new[] { "Roles" })]
        public async Task<IActionResult> Assign(Guid roleId, Guid permId)
        {
            await _svc.AssignPermissionAsync(roleId, permId);
            return Ok();
        }
    }
}
