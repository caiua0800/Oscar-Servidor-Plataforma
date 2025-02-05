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

        private readonly ConnectionIpService _connectionIpService;

        public AuthController(AuthService authService, ConnectionIpService connectionIpService)
        {
            _authService = authService;
            _connectionIpService = connectionIpService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromBody] UserLogin login)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _connectionIpService.RegisterIpAsync(userIp);
            Console.WriteLine($"IP registrado: {userIp}");
            return await _authService.GenerateTokenAsync(login);
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

        [HttpGet("me")]
        public async Task<IActionResult> GetAdminByToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var admin = await _authService.GetAdminByToken(token);
            if (admin == null)
            {
                return NotFound("Admin não encontrado.");
            }

            return Ok(admin);
        }

        [HttpGet("verify-permission/{permission}")]
        public async Task<IActionResult> GetAdminByToken(string permission)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var admin = await _authService.GetAdminByToken(token);
            if (admin == null)
            {
                return NotFound("Admin não encontrado.");
            }

            if (admin.PermissionLevel.Contains(permission) || admin.PermissionLevel.Contains("All"))
            {
                return Ok("O Admin Tem Permissão.");
            }
            else
            {
                return Forbid("O Admin Não Possui Permissão");
            }
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