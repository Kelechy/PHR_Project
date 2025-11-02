using Microsoft.AspNetCore.Mvc;
using PHR.Api.DTOs;
using PHR.Api.Services;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace PHR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register a new user", Tags = new[] { "Auth" })]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var res = await _auth.RegisterAsync(dto);
                return Ok(res);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login", Tags = new[] { "Auth" })]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var res = await _auth.LoginAsync(dto);
            if (res == null) return Unauthorized();
            return Ok(res);
        }

        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Refresh JWT", Tags = new[] { "Auth" })]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            var res = await _auth.RefreshTokenAsync(token);
            if (res == null) return Unauthorized();
            return Ok(res);
        }
    }
}
