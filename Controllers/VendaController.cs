using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers
{
    [Authorize(Roles = "Client, Admin")]
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
        [Authorize(Roles = "Client, Admin")]
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
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> GetAll()
        {
            var vendas = await _vendaService.GetAllVendasAsync();
            return Ok(vendas);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client, Admin")]
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
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> GetVendaByClientId(string id)
        {
            var venda = await _vendaService.GetVendaByClientIdAsync(id);
            if (venda == null)
            {
                return NotFound();
            }
            return Ok(venda);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Client, Admin")]
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
