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
            var existsVariable = await _systemConfigService.GetSystemConfigByNameAsync(systemConfig.Name);

            if (existsVariable != null)
            {
                return StatusCode(460, $"Já existe uma configuração com esse nome.");
            }

            var createdSystemConfig = await _systemConfigService.CreateSystemConfigAsync(systemConfig);
            return CreatedAtAction(nameof(GetSystemConfigByName), new { name = createdSystemConfig.Name }, createdSystemConfig);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var systemConfigs = await _systemConfigService.GetAllSystemConfigsAsync();
        return Ok(systemConfigs);
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteSystemConfigByName(string name)
    {
        var existsVariable = await _systemConfigService.GetSystemConfigByNameAsync(name);
        if (existsVariable == null)
        {
            return NotFound($"Configuração com o nome '{name}' não encontrada.");
        }

        try
        {
            await _systemConfigService.DeleteSystemConfigByNameAsync(name);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPut("{name}/{newValue}")]
    public async Task<IActionResult> EditSystemConfig(string name, string newValue)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(newValue))
            return BadRequest("Envie todas as informações para utilizar a rota.");

        var updatedConfig = await _systemConfigService.EditSystemConfigByName(name, newValue);

        if (updatedConfig == null)
        {
            return NotFound("Configuração não encontrada.");
        }

        return Ok(updatedConfig);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetSystemConfigByName(string name)
    {
        var purchase = await _systemConfigService.GetSystemConfigByNameAsync(name);
        if (purchase == null)
        {
            return NoContent();
        }
        return Ok(purchase);
    }

}
