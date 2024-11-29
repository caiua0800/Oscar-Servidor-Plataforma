using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using DotnetBackend.Services;
using System.Threading.Tasks;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ClientService _clientService;
        private readonly AdminService _adminService;

        public AuthController(IConfiguration configuration, ClientService clientService, AdminService adminService)
        {
            _configuration = configuration;
            _clientService = clientService;
            _adminService = adminService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromBody] UserLogin login)
        {
            // Tenta buscar o cliente pelo ID (CPF)
            var client = await _clientService.GetClientByIdAsync(login.Id);

            if (client == null)
            {
                // Se não encontrar o cliente, tenta buscar o administrador
                var admin = await _adminService.GetAdminByIdAsync(login.Id);

                // Verifica a senha do administrador
                if (admin != null && await _adminService.VerifyPasswordAsync(admin.Id, login.Password))
                {
                    return GenerateTokenResponse(admin.Name, admin.Id, "Admin", admin.PlatformId);
                }
            }
            else if (await _clientService.VerifyPasswordAsync(client.Id, login.Password))
            {
                // Aqui verificamos se o status do cliente permite o acesso
                if (client.Status == 2) // Supondo que Status representa acesso negado
                {
                    return Forbid("Acesso negado. O status do cliente não permite acesso.");
                }

                // Se a senha estiver correta, gera o token para o cliente
                return GenerateTokenResponse(client.Name, client.Id, "Client", client.PlatformId);
            }

            // Se nada for encontrado, retorna 401 Unauthorized
            return Unauthorized();
        }


        private IActionResult GenerateTokenResponse(string name, string id, string role, string platformId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, role),
                new Claim("PlatformId", platformId)
            };

            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                return Problem("A chave JWT não está configurada.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                platformId = platformId // Retorna o PlatformId no resultado do token
            });
        }

    }

    public class UserLogin
    {
        public required string Id { get; set; } // CPF do cliente ou ID do administrador
        public required string Password { get; set; }
    }
}
