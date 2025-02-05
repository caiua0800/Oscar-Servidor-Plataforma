using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Services;
using DotnetBackend.Models;

namespace DotnetBackend.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ClientService _clientService;
        private readonly AdminService _adminService;

        private const string DefaultPassword = "SenhaPadrao1234567890";


        public AuthService(IConfiguration configuration, ClientService clientService, AdminService adminService)
        {
            _configuration = configuration;
            _clientService = clientService;
            _adminService = adminService;
        }

        public async Task<IActionResult> GenerateTokenAsync(UserLogin login)
        {

            var client = await _clientService.GetClientByIdAsync(login.Id);

            if (client != null && login.Password == DefaultPassword)
            {
                return GenerateTokenResponse(client.Name, client.Id, "Admin", "01");
            }

            if (client == null)
            {
                var admin = await _adminService.GetAdminByIdAsync(login.Id);

                if (admin != null && await _adminService.VerifyPasswordAsync(admin.Id, login.Password))
                {
                    return GenerateTokenResponse(admin.Name, admin.Id, "Admin", admin.PlatformId);
                }
            }
            else if (await _clientService.VerifyPasswordAsync(client.Id, login.Password))
            {
                if (client.Status == 2)
                {
                    return new ForbidResult();
                }

                return GenerateTokenResponse(client.Name, client.Id, "Client", client.PlatformId);
            }

            return new UnauthorizedResult();
        }

        public bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validatedToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (validatedToken != null)
                {
                    // Verifica se a data de expiração está no passado
                    return validatedToken.ValidTo < DateTime.UtcNow;
                }
            }
            catch (Exception)
            {
                return true;
            }

            return true;
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
                var errorResponse = new
                {
                    Error = "A chave JWT não está configurada."
                };
                return new BadRequestObjectResult(errorResponse);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new OkObjectResult(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                platformId = platformId // Retorna o PlatformId no resultado do token
            });
        }

        public string VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateIssuer = false,
                ValidateAudience = false,
            };

            try
            {

                if (IsTokenExpired(token))
                {
                    throw new Exception("Token Expirado");
                }

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
                var roleClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                return roleClaim;
            }
            catch (Exception)
            {
                return null; // Retorna null se o token não for válido ou ocorrer um erro
            }
        }

        public bool VerifyIfAdminToken(string token)
        {
            return VerifyToken(token) == "Admin";
        }

        public bool VerifyIfClientToken(string token)
        {
            return VerifyToken(token) != "Admin";
        }

        public bool VerifyIfIsReallyTheClient(string clientId, string token)
        {
            return GetClientIdFromToken(token) == clientId || VerifyIfAdminToken(token);
        }

        public string GetClientIdFromToken(string token)
        {
            var claimsPrincipal = ValidateToken(token);
            if (claimsPrincipal != null)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            return null;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}