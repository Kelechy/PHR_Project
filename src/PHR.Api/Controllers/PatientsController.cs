using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PHR.Api.Models;
using PHR.Api.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace PHR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _svc;

        public PatientsController(IPatientService svc)
        {
            _svc = svc;
        }

        //private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        private Guid GetUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idValue))
                throw new UnauthorizedAccessException("User ID claim missing or invalid.");

            return Guid.Parse(idValue);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create patient record", Tags = new[] { "Patients" })]
        public async Task<IActionResult> Create(PatientRecord dto)
        {
            var userId = GetUserId();

            var canCreate = User.HasPermission("createPatientRecords");
            if (!canCreate)
                return Forbid();

            dto.CreatedBy = userId;
            var created = await _svc.CreateAsync(dto);
            return Ok(created);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get patient record by id", Tags = new[] { "Patients" })]
        public async Task<IActionResult> Get(Guid id)
        {
            var userId = GetUserId();

            var canView = await _svc.CanViewAsync(userId, id);
            if (!canView)
                return Forbid();

            var rec = await _svc.GetAsync(id);
            if (rec == null)
                return NotFound();

            return Ok(rec);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Get accessible records", Tags = new[] { "Patients" })]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var list = await _svc.GetAllAccessibleAsync(userId);
            return Ok(list);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Update own record", Tags = new[] { "Patients" })]
        public async Task<IActionResult> Update(Guid id, PatientRecord dto)
        {
            var userId = GetUserId();
            dto.Id = id;

            var updated = await _svc.UpdateAsync(dto, userId);
            if (updated == null)
                return Forbid();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Soft delete own record", Tags = new[] { "Patients" })]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();

            var ok = await _svc.SoftDeleteAsync(id, userId);
            if (!ok)
                return Forbid();

            return NoContent();
        }
    }
}
