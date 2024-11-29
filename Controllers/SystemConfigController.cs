using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemConfigController : ControllerBase
{
    private readonly SystemConfigService _systemConfigService;

    public SystemConfigController(SystemConfigService systemConfig)
    {
        _systemConfigService = systemConfig;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SystemConfig systemConfig)
    {
        if (systemConfig == null)
        {
            return BadRequest("System Config is null.");
        }

        try
        {
            var createdSystemConfig = await _systemConfigService.CreateSystemConfigAsync(systemConfig);
            return CreatedAtAction(nameof(GetSystemConfigByName), new { name = createdSystemConfig.Name }, createdSystemConfig);
        }
        catch (Exception ex)
        {
            // Log the exception (consider using a logging framework)
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var systemConfigs = await _systemConfigService.GetAllSystemConfigsAsync();
        return Ok(systemConfigs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSystemConfigByName(string name)
    {
        var purchase = await _systemConfigService.GetSystemConfigByNameAsync(name);
        if (purchase == null)
        {
            return NotFound();
        }
        return Ok(purchase);
    }

}
