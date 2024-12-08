using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WithdrawalController : ControllerBase
{
    private readonly WithdrawalService _withdrawalService;

    public WithdrawalController(WithdrawalService withdrawalService)
    {
        _withdrawalService = withdrawalService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Withdrawal withdrawal)
    {
        if (withdrawal == null)
        {
            return BadRequest("Withdraw is null.");
        }

        Console.WriteLine("Criando Saque");
        var createdWithdrawal = await _withdrawalService.CreateWithdrawalAsync(withdrawal);

        if (string.IsNullOrEmpty(createdWithdrawal.WithdrawalId))
        {
            return BadRequest("WithdrawalId não foi gerado.");
        }

        return CreatedAtAction(nameof(GetWithdrawalById), new { id = createdWithdrawal.WithdrawalId }, createdWithdrawal);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWithdrawalById(string id)
    {
        var withdrawal = await _withdrawalService.GetWithdrawalByIdAsync(id);
        if (withdrawal == null)
        {
            return NotFound();
        }
        return Ok(withdrawal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _withdrawalService.DeleteWithdrawalAsync(id);
        if (!result)
        {
            Console.WriteLine($"Withdrawal with ID {id} not found.");
            return NotFound();
        }
        return NoContent();
    }

    [HttpPut("{id}/{newStatus}")]
    public async Task<IActionResult> UpdateStatus(string id, int newStatus)
    {
        var result = await _withdrawalService.UpdateStatus(id, newStatus);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("toPay")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllWithdrawsToPay()
    {
        List<Withdrawal> withdrawals = await _withdrawalService.GetAllWithdrawsToPay();

        if (withdrawals == null || withdrawals.Count == 0)
        {
            return NotFound("Nenhum saque encontrado com status 1."); // Retorna 404 se não encontrar saques
        }

        double totalAmount = withdrawals.Sum(w => w.AmountWithdrawn - (w.AmountWithdrawn * 0.04));

        var response = new
        {
            TotalAmount = totalAmount,
            Withdrawals = withdrawals
        };

        return Ok(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var withdrawals = await _withdrawalService.GetAllWithdrawalsAsync();
        return Ok(withdrawals);
    }
}
