using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly VendaService _vendaService;

        public VendaController(VendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Venda venda)
        {
            if (venda == null)
            {
                return BadRequest("Venda is null.");
            }

            var createdVenda = await _vendaService.CreateVendaAsync(venda);
            return CreatedAtAction(nameof(GetVendaById), new { id = createdVenda.Id }, createdVenda);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vendas = await _vendaService.GetAllVendasAsync();
            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendaById(string id)
        {
            var venda = await _vendaService.GetVendaByIdAsync(id);
            if (venda == null)
            {
                return NotFound();
            }
            return Ok(venda);
        }

        [HttpGet("client/{id}")]
        public async Task<IActionResult> GetVendaByClientId(string id)
        {
            var venda = await _vendaService.GetVendasByClientIdAsync(id);
            if (venda == null)
            {
                return NoContent();
            }
            return Ok(venda);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _vendaService.DeleteVendaAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
