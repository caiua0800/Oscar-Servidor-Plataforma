using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly PurchaseService _purchaseService;

        public PurchaseController(PurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Purchase purchase)
        {
            if (purchase == null)
            {
                return BadRequest("Purchase is null.");
            }

            var createdPurchase = await _purchaseService.CreatePurchaseAsync(purchase);
            return CreatedAtAction(nameof(GetPurchaseById), new { id = createdPurchase.PurchaseId }, createdPurchase);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases); // Retorna a lista de compras
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseById(string id)
        {
            var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            return Ok(purchase);
        }

        [HttpGet("client/{id}")]
        public async Task<IActionResult> GetPurchasesByClientId(string id)
        {
            var purchases = await _purchaseService.GetPurchasesByClientIdAsync(id);
            if (purchases == null || purchases.Count == 0)
            {
                return NotFound();
            }
            return Ok(purchases);
        }

        [HttpGet("verify/{id}/{ticketId}")]
        public async Task<IActionResult> VerifyPayment(string id, string ticketId)
        {
            bool isPaymentVerified = await _purchaseService.VerifyPayment(id, ticketId);
            if (isPaymentVerified)
            {
                return Ok(new
                {
                    message = "Pagamento verificado com sucesso.",
                    paid = true
                });
            }
            else
            {
                return NotFound(new
                {
                    message = "Pagamento não encontrado ou não verificado.",
                    paid = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _purchaseService.DeletePurchaseAsync(id);
            if (!result)
            {
                return NotFound(); // Retorna 404 se a compra não for encontrada
            }
            return NoContent(); // Retorna 204 se a exclusão foi bem-sucedida
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Purchase newPurchase)
        {
            var result = await _purchaseService.UpdatePurchaseAsync(id, newPurchase);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/{newStatus}")]
        public async Task<IActionResult> UpdateStatus(string id, int newStatus)
        {
            var result = await _purchaseService.UpdateStatus(id, newStatus);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/add/{amount}")]
        public async Task<IActionResult> AddIncrement(string id, decimal amount)
        {
            var result = await _purchaseService.AddIncrementToPurchaseAsync(id, amount);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelPurchase(string id)
        {
            var result = await _purchaseService.CancelPurchase(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/anticipate-profit/{increasement}")]
        public async Task<IActionResult> AnticipateProfit(string id, decimal increasement)
        {
            var result = await _purchaseService.AnticipateProfit(id, increasement);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLast50Extracts()
        {
            var purchases = await _purchaseService.GetLast50PurchasesAsync();
            if (purchases == null || purchases.Count == 0)
            {
                return NotFound();
            }

            var orderedPurchases = purchases.OrderByDescending(e => e.PurchaseId).ToList();

            return Ok(orderedPurchases);
        }
    }
}