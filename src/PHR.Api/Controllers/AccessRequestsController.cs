using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PHR.Api.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using PHR.Api.DTOs;

namespace PHR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessRequestsController : ControllerBase
    {
        private readonly IAccessRequestService _svc;
        public AccessRequestsController(IAccessRequestService svc) { _svc = svc; }

        //private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        private Guid GetUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idValue))
                throw new UnauthorizedAccessException("User ID claim missing or invalid.");

            return Guid.Parse(idValue);
        }

        [HttpPost("{patientId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Request access to a patient record", Tags = new[] { "AccessRequests" })]
        public async Task<IActionResult> RequestAccess(Guid patientId, [FromBody] string reason)
        {
            var userId = GetUserId();
            var r = await _svc.RequestAccessAsync(patientId, userId, reason);
            return Ok(r);
        }

        [HttpGet("pending")]
        [Authorize]
        [SwaggerOperation(Summary = "Get pending access requests", Tags = new[] { "AccessRequests" })]
        public async Task<IActionResult> GetPending()
        {
            var list = await _svc.GetPendingAsync();
            return Ok(list);
        }

        [HttpPost("{id}/approve")]
        [Authorize]
        [SwaggerOperation(Summary = "Approve access request", Tags = new[] { "AccessRequests" })]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _svc.ApproveAsync(id, GetUserId(), dto.Start, dto.End);
            if (!ok) return BadRequest();
            return Ok();
        }

        [HttpPost("{id}/decline")]
        [Authorize]
        [SwaggerOperation(Summary = "Decline access request", Tags = new[] { "AccessRequests" })]
        public async Task<IActionResult> Decline(Guid id)
        {
            var ok = await _svc.DeclineAsync(id, GetUserId());
            if (!ok) return BadRequest();
            return Ok();
        }
       
    }
}
