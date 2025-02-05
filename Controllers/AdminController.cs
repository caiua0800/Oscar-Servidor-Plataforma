using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly WithdrawalService _withdrawalService;
        private readonly AuthService _authService;
        private readonly ConnectionIpService _connectionIpService;


        public AdminController(AdminService adminService, AuthService authService, ConnectionIpService connectionIpService)
        {
            _adminService = adminService;
            _authService = authService;
            _connectionIpService = connectionIpService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] Admin admin)
        {
            if (admin == null || string.IsNullOrEmpty(admin.Password))
            {
                return BadRequest("Admin ou a senha está nula.");
            }

            var createdAdmin = await _adminService.CreateAdminAsync(admin, admin.Password);
            return CreatedAtAction(nameof(CreateAdmin), new { id = createdAdmin.Id }, createdAdmin);
        }

        [HttpPut("{id}/permissions/{newPermission}")]
        public async Task<IActionResult> UpdateAdminPermissions(string id, string newPermission)
        {
            if (newPermission == null || string.IsNullOrEmpty(newPermission))
            {
                return BadRequest("As permissões de admin é nula.");
            }

            var createdAdmin = await _adminService.ChangePermissions(id, newPermission);
            return createdAdmin ? Ok("Permissões Editadas Com Sucesso.") : BadRequest("Não foi possível editar");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _adminService.GetAllAdminsAsync();
            Console.WriteLine("Admins no banco de dados: ");
            foreach (var admin in admins)
            {
                Console.WriteLine($"Id (CPF): {admin.Id}, Name: {admin.Name}");
            }
            return Ok(admins);
        }

        [HttpGet("weekly-views")]
        public async Task<IActionResult> GetLastWeekAccess()
        {
            var access = await _connectionIpService.CountTotalAccessesLastWeekAsync();
            if(access == 0)
            {
                return Ok("Sem Dados.");
            }
            return Ok(access);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            Console.WriteLine($"Buscando cliente com CPF: '{id}'");
            var admin = await _adminService.GetAdminByIdAsync(id);
            if (admin == null)
            {
                Console.WriteLine($"Cliente com CPF '{id}' não encontrado.");
                return NotFound(); // Retorna 404 se o admin não for encontrado
            }
            return Ok(admin); // Retorna o admin encontrado
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAdminDetailsById()
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é ela");
            }

            var admin = await _authService.GetAdminByToken(token);

            if (admin == null)
            {
                Console.WriteLine($"Admin com Id '{admin.Id}' não encontrado.");
                return NotFound();
            }


            if (admin.PermissionLevel.Contains("All"))
            {
                admin.Permission = "Superior";
                Console.WriteLine($"HIHI: AdminPermissions {admin.PermissionLevel}");
            }
            else if (admin.PermissionLevel.Contains("Contrato, Saque"))
            {
                admin.Permission = "Intermediário";
            }
            else
            {
                admin.Permission = "Baixo";
            }

            return Ok(admin);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _adminService.DeleteAdminAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
