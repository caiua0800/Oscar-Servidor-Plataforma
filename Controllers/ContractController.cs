using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly ContractService _contractService;

        public ContractController(ContractService contractService)
        {
            _contractService = contractService;
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
                return NotFound(); // Retorna 404 se não encontrar o contrato
            }
            return Ok(contract); // Retorna o contrato encontrado
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContracts()
        {
            var contracts = await _contractService.GetAllContractsAsync();
            return Ok(contracts); // Retorna a lista de contratos
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateField(string id, [FromBody] ContractModel updatedModel)
        {

            if (updatedModel == null)
            {
                return BadRequest("Modelo de contrato não pode ser nulo.");
            }

            // O ID no novo modelo deve corresponder ao ID fornecido na URL
            updatedModel.Id = id;

            var result = await _contractService.ReplaceContractAsync(id, updatedModel);

            if (!result)
            {
                return NotFound(); // Retorna 404 se o contrato não for encontrado
            }
            return NoContent(); // Retorna 204 se a atualização for bem-sucedida
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _contractService.DeleteContractAsync(id);
            if (!result)
            {
                return NotFound(); // Retorna 404 se o contrato não for encontrado
            }
            return NoContent(); // Retorna 204 se a exclusão for bem-sucedida
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ContractModel contractModel)
        {
            if (contractModel == null)
            {
                return BadRequest("Modelo de contrato não pode ser nulo.");
            }

            var result = await _contractService.UpdateContractAsync(id, contractModel);
            if (!result)
            {
                return NotFound(); // Retorna 404 se o contrato não for encontrado
            }
            return NoContent(); // Retorna 204 se a atualização for bem-sucedida
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
}
