using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly BankAccountService _bankAccountService;
        private readonly ExtractService _extractService;

        public BankAccountController(BankAccountService bankAccountService, ExtractService extractService)
        {
            _bankAccountService = bankAccountService;
            _extractService = extractService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BankAccount bankAccount)
        {
            if (bankAccount == null)
            {
                return BadRequest("Conta bancária não pode ser null.");
            }

            try
            {
                var createdBankAccount = await _bankAccountService.CreateBankAccountAsync(bankAccount);
                return CreatedAtAction(nameof(GetBankAccountAsync), createdBankAccount);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao criar a conta bancária: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBankAccountAsync()
        {
            var bankAccount = await _bankAccountService.GetBankAccountAsync();
            if (bankAccount == null)
            {
                return NotFound("Conta bancária não encontrada.");
            }
            return Ok(bankAccount);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawFromBalance([FromBody] WithdrawRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("O valor do saque deve ser positivo.");
            }

            try
            {
                var updatedBankAccount = await _bankAccountService.WithdrawFromBalanceAsync(request.WithdrawId, request.Amount);
                return Ok(updatedBankAccount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao realizar o saque: " + ex.Message);
            }
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> AddToBalance([FromBody] decimal amount, string purchaseId)
        {
            if (amount <= 0)
            {
                return BadRequest("O valor do depósito deve ser positivo.");
            }

            try
            {
                if (purchaseId == null)
                {
                    var updatedBankAccount = await _bankAccountService.AddToBalanceAsync(null, amount);
                    return Ok(updatedBankAccount);
                }
                else
                {
                    var updatedBankAccount = await _bankAccountService.AddToBalanceAsync(purchaseId, amount);
                    return Ok(updatedBankAccount);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao realizar o depósito na conta bancária: " + ex.Message);
            }
        }

        [HttpGet("extracts/search")]
        public async Task<IActionResult> GetExtractsContainingString([FromQuery] string searchString)
        {
            try
            {
                var extracts = await _extractService.GetExtractsContainingStringAsync(searchString);
                return Ok(extracts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao buscar extratos: " + ex.Message);
            }
        }
    }

    public class WithdrawRequest
    {
        public string? WithdrawId { get; set; }
        public decimal Amount { get; set; }
    }
}