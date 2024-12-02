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

        [HttpGet("withdraw")]
        public async Task<IActionResult> GetAllWithdrawals()
        {
            var withdrawals = await _gatewayService.GetAllWithdrawals();
            return Ok(withdrawals);
        }

        [HttpGet("adminwithdraw")]
        public async Task<IActionResult> GetAllAdminWithdrawals()
        {
            var withdrawals = await _gatewayService.GetAllAdminWithdrawals();
            return Ok(withdrawals);
        }

    }
}