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

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
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
