using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DotnetBackend.Queries;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultorController : ControllerBase
    {
        private readonly ClientService _clientService;
        private readonly ConsultorService _consultorService;
        private readonly AuthService _authService;

        public ConsultorController(ClientService clientService, AuthService authService, ConsultorService consultorService)
        {
            _clientService = clientService;
            _authService = authService;
            _consultorService = consultorService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Consultor consultor)
        {
            Console.WriteLine(consultor.Name);

            if (consultor == null)
            {
                return BadRequest("Consultor null.");
            }

            var createdConsultor = await _consultorService.CreateConsultorAsync(consultor);

            Console.WriteLine($"Consultor criado: {createdConsultor.Id}, {createdConsultor.Name}");

            return CreatedAtAction(nameof(Create), new { id = createdConsultor.Id }, createdConsultor);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é ela");
            }

            var consultoresWithClientCount = await _consultorService.GetAllConsultoresAsync();

            return Ok(consultoresWithClientCount);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é ela");
            }

            var result = await _consultorService.DeleteConsultorAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConsultorById(string id)
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é ela");
            }

            var consultor = await _consultorService.GetConsultorByIdAsync(id);
            if (consultor == null)
            {
                Console.WriteLine($"Consultor com CPF '{id}' não encontrado.");
                return NotFound();
            }
            return Ok(consultor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Consultor updatedClient)
        {
            if (updatedClient == null)
            {
                return BadRequest("O consultor atualizado não pode ser nulo.");
            }

            var result = await _consultorService.UpdateConsultorAsync(id, updatedClient);
            if (!result)
            {
                return NotFound($"Consultor com id '{id}' não encontrado.");
            }

            var client = await _consultorService.GetConsultorByIdAsync(id);
            if (client == null)
            {
                return NotFound($"Consultor com id '{id}' não encontrado após atualização.");
            }

            return Ok(client);
        }

        [HttpPut("{id}/email")]
        public async Task<IActionResult> UpdateConsultorEmail(string id, [FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("O Email não pode ser vazio.");
            }

            try
            {
                var result = await _consultorService.UpdateConsultorEmail(id, email);
                if (!result)
                    return NotFound("O consultor não foi encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar o email: {ex.Message}");
            }
        }

        [HttpPut("{id}/phone/{newPhone}")]
        public async Task<IActionResult> UpdateConsultorPhone(string id, string newPhone)
        {
            if (string.IsNullOrEmpty(newPhone))
            {
                return BadRequest("O telefone não pode ser vazio.");
            }

            try
            {
                var result = await _consultorService.UpdateConsultorPhone(id, newPhone);
                if (!result)
                {
                    return NotFound($"Consultor com ID '{id}' não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar o telefone: {ex.Message}");
            }
        }

    }
}
