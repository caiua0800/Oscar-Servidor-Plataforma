using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtractController : ControllerBase
    {
        private readonly ExtractService _extractService; // Corrigido para usar ExtractService

        public ExtractController(ExtractService extractService) // Alterado para Extrato
        {
            _extractService = extractService;
        }

        // Método para obter todos os extratos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var extracts = await _extractService.GetAllExtractsAsync(); // Método para obter extratos
            return Ok(extracts); // Retorna a lista de extratos
        }

        // Método para obter um extrato pela ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> GetExtractById(string id)
        {
            var extract = await _extractService.GetExtractByIdAsync(id); 
            if (extract == null)
            {

                return NotFound(); // Retorna 404 se o extrato não for encontrado
            }
            return Ok(extract); // Retorna o extrato encontrado
        }

        // Método para obter extratos por ClientId
        [HttpGet("client/{id}")]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> GetExtractsByClientId(string id)
        {
            var extracts = await _extractService.GetExtractsByClientIdAsync(id); // Método para obter extratos por clientId
            if (extracts == null || extracts.Count == 0) // Verifica se a lista está vazia ou é nula
            {
                return NotFound(); // Retorna 404 se não houver extratos
            }
            return Ok(extracts); // Retorna a lista de extratos
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _extractService.DeleteExtractAsync(id); // Método para excluir extrato
            if (!result)
            {
                return NotFound(); // Retorna 404 se o extrato não for encontrado
            }
            return NoContent();
        }

        [HttpGet("client/{id}/last")]
        public async Task<IActionResult> GetLastExtractsByClientId(string id)
        {
            var extracts = await _extractService.GetLastExtractsByClientIdAsync(id);
            if (extracts == null || extracts.Count == 0)
            {
                return NotFound();
            }
            return Ok(extracts);
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLast50Extracts()
        {
            var extracts = await _extractService.GetLast50ExtractsAsync();
            if (extracts == null || extracts.Count == 0)
            {
                return NotFound();
            }

            var orderedExtracts = extracts.OrderByDescending(e => e.ExtractId).ToList();

            return Ok(orderedExtracts);
        }


    }
}
