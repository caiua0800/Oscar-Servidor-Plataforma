using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminWithdrawalController : ControllerBase
{
    private readonly AdminWithdrawalService _adminWithdrawalService;

    public AdminWithdrawalController(AdminWithdrawalService adminWithdrawalService)
    {
        _adminWithdrawalService = adminWithdrawalService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminWithdrawal adminWithdraw)
    {
        if (adminWithdraw == null)
        {
            return BadRequest("adminWithdraw is null.");
        }

        var createdAdminWithdrawal = await _adminWithdrawalService.CreateWithdrawalAsync(adminWithdraw);
        return CreatedAtAction(nameof(GetAdminWithdrawalById), new { id = createdAdminWithdrawal.AdminWithdrawalId }, createdAdminWithdrawal);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAdminWithdrawalById(string id)
    {
        var withdrawal = await _adminWithdrawalService.GetAdminWithdrawalByIdAsync(id);
        if (withdrawal == null)
        {
            return NotFound();
        }
        return Ok(withdrawal);
    }

    [HttpPut("{id}/{newStatus}")]
    public async Task<IActionResult> UpdateStatus(string id, int newStatus)
    {
        var result = await _adminWithdrawalService.UpdateStatus(id, newStatus);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("toPay")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdminWithdrawsToPay()
    {
        List<AdminWithdrawal> withdrawals = await _adminWithdrawalService.GetAllAdminWithdrawsToPay();

        if (withdrawals == null || withdrawals.Count == 0)
        {
            return NotFound("Nenhum saque de admin encontrado com status 1."); // Retorna 404 se não encontrar saques
        }

        double totalAmount = withdrawals.Sum(w => (w.AmountWithdrawn * 0.96));

        var response = new
        {
            TotalAmount = totalAmount,
            AdminWithdrawals = withdrawals
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var withdrawals = await _adminWithdrawalService.GetAllAdminWithdrawalsAsync();
        return Ok(withdrawals);
    }
}
