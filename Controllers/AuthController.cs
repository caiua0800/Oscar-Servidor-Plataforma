using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DotnetBackend.Services;
using DotnetBackend.Models;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromBody] UserLogin login)
        {
            return await _authService.GenerateTokenAsync(login); // Delegando a chamada para o AuthService
        }

        [HttpGet("verify/{token}")]
        public async Task<IActionResult> VerifyToken(string token)
        {
            if (token == null)
            {
                return BadRequest("Envie o Token para a verificação.");
            }

            var response = _authService.VerifyToken(token);


            return Ok(response);
        }

        [HttpGet("verify-if/{token}")]
        public async Task<IActionResult> VerifyClientIdByToken(string token)
        {
            if (token == null)
            {
                return BadRequest("Envie o Token para a verificação.");
            }

            var response = _authService.GetClientIdFromToken(token);

            return Ok(response);
        }
    }
}