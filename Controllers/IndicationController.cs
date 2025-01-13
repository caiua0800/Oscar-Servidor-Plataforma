using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace DotnetBackend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class IndicationController : ControllerBase
{
    private readonly IndicationService _indicationService;
    private readonly ClientService _clientService;

    public IndicationController(IndicationService indicationService, ClientService clientService)
    {
        _indicationService = indicationService;
        _clientService = clientService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Indication indication)
    {
        if (indication == null)
        {
            return BadRequest("Indicação não pode ser nula.");
        }

        var createdIndication = await _indicationService.CreateIndicationAsync(indication);
        await _clientService.AddToExtraBalanceAsync(indication.ClientId, (decimal)indication.Value);

        return CreatedAtAction(nameof(GetIndicationById), new { id = createdIndication.Id }, createdIndication); 
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIndicationById(string id)
    {
        var indication = await _indicationService.GetIndicationByIdAsync(id);
        if (indication == null)
        {
            return NotFound();
        }
        return Ok(indication);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllIndications()
    {
        var indications = await _indicationService.GetAllIndicationsAsync();
        return Ok(indications);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _indicationService.DeleteIndicationAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
