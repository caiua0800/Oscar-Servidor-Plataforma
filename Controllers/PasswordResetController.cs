using DotnetBackend.Services;
using DotnetBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly PasswordResetService _passwordResetService;
        private readonly TokenService _tokenService;
        private readonly ClientService _clientService;
        private readonly AuthService _authService;

        public PasswordResetController(EmailService emailService, PasswordResetService passwordResetService, TokenService tokenService, ClientService clientService, AuthService authService)
        {
            _emailService = emailService;
            _passwordResetService = passwordResetService;
            _tokenService = tokenService;
            _clientService = clientService;
            _authService = authService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email é obrigatório.");
            }

            await _passwordResetService.SendPasswordResetEmail(request.Email);
            return Ok("Email enviado para redefinição de senha.");
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Token e nova senha são obrigatórios.");
            }

            var tokenData = await _tokenService.GetTokenData(request.Token);
            if (tokenData == null || tokenData.CreatedAt.AddMinutes(30) < DateTime.UtcNow)
            {
                return BadRequest("Token inválido ou expirado.");
            }

            var user = await _clientService.GetClientByIdAsync(tokenData.UserId);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            await _clientService.UpdatePasswordAsync(user.Id, request.NewPassword);
            await _tokenService.DeleteToken(tokenData.Token);

            return Ok("Senha redefinida com sucesso!");
        }

        [HttpPost("reset-admin/{clientId}/{newPassword}")]
        public async Task<IActionResult> ResetPasswordAdmin(string clientId, string newPassword)
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            string roleClaim = _authService.VerifyToken(token);

            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid();
            }

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("clientId e nova senha são obrigatórios.");
            }

            var user = await _clientService.GetClientByIdAsync(clientId);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            await _clientService.UpdatePasswordAsync(clientId, newPassword);
            return Ok("Senha redefinida com sucesso!");
        }
    }
}