using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace DotnetBackend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ContractController : ControllerBase
{
    private readonly ContractService _contractService;
    private readonly AuthService _authService;

    public ContractController(ContractService contractService, AuthService authService)
    {
        _contractService = contractService;
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContractModel contractModel)
    {
        if (contractModel == null)
        {
            return BadRequest("Modelo de contrato não pode ser nulo.");
        }

        var createdContract = await _contractService.CreateContractAsync(contractModel);
        return CreatedAtAction(nameof(GetContractById), new { id = createdContract.Id }, createdContract); // Retorna o contrato criado
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContractById(string id)
    {
        var contract = await _contractService.GetContractByIdAsync(id);
        if (contract == null)
        {
            return NotFound();
        }
        return Ok(contract);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContracts()
    {
        var contracts = await _contractService.GetAllContractsAsync();
        return Ok(contracts);
    }

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> UpdateField(string id, [FromBody] ContractModel updatedModel)
    {

        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var token = authorizationHeader.ToString().Replace("Bearer ", "");
        if (!_authService.VerifyIfAdminToken(token))
        {
            return Forbid("Você não é ela");
        }

        if (updatedModel == null)
        {
            return BadRequest("Modelo de contrato não pode ser nulo.");
        }

        updatedModel.Id = id;

        var result = await _contractService.ReplaceContractAsync(id, updatedModel);

        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _contractService.DeleteContractAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ContractModel contractModel)
    {

        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var token = authorizationHeader.ToString().Replace("Bearer ", "");
        if (!_authService.VerifyIfAdminToken(token))
        {
            return Forbid("Você não é ela");
        }

        if (contractModel == null)
        {
            return BadRequest("Modelo de contrato não pode ser nulo.");
        }

        var result = await _contractService.UpdateContractAsync(id, contractModel);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}

public class UpdateFieldModel
{
    public string FieldName { get; set; }
    public string StringValue { get; set; }
    public int? IntValue { get; set; }
    public double? DoubleValue { get; set; }
    public bool? BoolValue { get; set; }
}
