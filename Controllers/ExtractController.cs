using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtractController : ControllerBase
    {
        private readonly ExtractService _extractService;
        public ExtractController(ExtractService extractService) 
        {
            _extractService = extractService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var extracts = await _extractService.GetAllExtractsAsync();
            return Ok(extracts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> GetExtractById(string id)
        {
            var extract = await _extractService.GetExtractByIdAsync(id); 
            if (extract == null)
            {

                return NotFound(); 
            }
            return Ok(extract);
        }

        [HttpGet("client/{id}")]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> GetExtractsByClientId(string id)
        {
            var extracts = await _extractService.GetExtractsByClientIdAsync(id); 
            if (extracts == null || extracts.Count == 0) 
            {
                return NotFound(); 
            }
            return Ok(extracts); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _extractService.DeleteExtractAsync(id);
            if (!result)
            {
                return NotFound(); 
            }
            return NoContent();
        }

        [HttpGet("client/{id}/last")]
        public async Task<IActionResult> GetLastExtractsByClientId(string id)
        {
            var extracts = await _extractService.GetLastExtractsByClientIdAsync(id);
            if (extracts == null || extracts.Count == 0)
            {
                return NotFound();
            }
            return Ok(extracts);
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLast50Extracts()
        {
            var extracts = await _extractService.GetLast50ExtractsAsync();
            if (extracts == null || extracts.Count == 0)
            {
                return NotFound();
            }

            var orderedExtracts = extracts.OrderByDescending(e => e.ExtractId).ToList();

            return Ok(orderedExtracts);
        }


    }
}
