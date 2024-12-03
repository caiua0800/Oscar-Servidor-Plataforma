using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly GatewayService _gatewayService;

        public GatewayController(GatewayService gatewayService)
        {
            _gatewayService = gatewayService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var platformInfo = await _gatewayService.GetPlatformInfos();
            return Ok(platformInfo);
        }

        [HttpGet("withdraw/pending")]
        public async Task<IActionResult> GetAllWithdrawalsToPay()
        {
            var withdrawals = await _gatewayService.GetAllWithdrawalsToPay();
            return Ok(withdrawals);
        }

        [HttpGet("adminwithdraw/pending")]
        public async Task<IActionResult> GetAllAdminWithdrawalsToPay()
        {
            var withdrawals = await _gatewayService.GetAllAdminWithdrawalsToPay();
            return Ok(withdrawals);
        }

        [HttpGet("withdraw/paid")]
        public async Task<IActionResult> GetAllWithdrawalsPaid()
        {
            var withdrawals = await _gatewayService.GetAllWithdrawals();
            return Ok(withdrawals);
        }

        [HttpGet("adminwithdraw/paid")]
        public async Task<IActionResult> GetAllAdminWithdrawals()
        {
            var withdrawals = await _gatewayService.GetAllAdminWithdrawals();
            return Ok(withdrawals);
        }
    }
}