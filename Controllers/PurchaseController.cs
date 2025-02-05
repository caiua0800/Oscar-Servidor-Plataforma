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
        private readonly AuthService _authService;


        public PurchaseController(PurchaseService purchaseService, AuthService authService)
        {
            _purchaseService = purchaseService;
            _authService = authService;
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
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é admin");
            }

            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
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
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfIsReallyTheClient(id, token))
            {
                return Forbid("Você não é ela");
            }

            var purchases = await _purchaseService.GetPurchasesByClientIdAsync(id);
            if (purchases == null || purchases.Count == 0)
            {
                return NotFound();
            }
            return Ok(purchases);
        }

        [HttpGet("6monthsData")]
        public async Task<IActionResult> Get6MonthsData()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é ela");
            }

            var data = await _purchaseService.GetLastSixMonthsPurchaseDataAsync();
            if (data == null)
            {
                return NoContent();
            }
            return Ok(data);
        }


        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é ela");
            }

            var data = await _purchaseService.GetPurchaseSummaryForCurrentMonthAsync();
            if (data == null)
            {
                return NoContent();
            }
            return Ok(data);
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
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Purchase newPurchase)
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é admin");
            }

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

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é admin");
            }

            var result = await _purchaseService.UpdateStatus(id, newStatus);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/freeWithdraw/{freeStatus}")]
        public async Task<IActionResult> UpdateLiberarENegarSaque(string id, bool freeStatus)
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            
            bool verificado =  await _authService.VerifyPermissionAdmin(token, "ContratoEdit");

            if (!verificado)
            {
                return Forbid("Você não é permitido");
            }

            var result = await _purchaseService.UpdateFreeWithdraw(id, freeStatus);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("cancel-payment/{id}/{clientId}")]
        public async Task<IActionResult> CancelPaymentContract(string id, string clientId)
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");

            if (!_authService.VerifyIfIsReallyTheClient(clientId, token))
            {
                return Forbid("Você não é ela");
            }

            var result = await _purchaseService.CancelPaymentContract(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/add/{amount}")]
        public async Task<IActionResult> AddIncrement(string id, decimal amount)
        {
            var resultOne = await _purchaseService.GetPurchaseByIdAsync(id);
            if (resultOne == null)
            {
                return NotFound();
            }

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");

            if (!_authService.VerifyIfIsReallyTheClient(resultOne.ClientId, token))
            {
                Console.WriteLine(token);
                return Forbid("Você não é nada");
            }

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

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é admin");
            }

            var result = await _purchaseService.AnticipateProfit(id, increasement);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{id}/free-withdraw/{newStatus}")]
        public async Task<IActionResult> FreeWithdraw(string id, string newStatus)
        {

            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            if (!_authService.VerifyIfAdminToken(token))
            {
                return Forbid("Você não é admin");
            }

            bool status = newStatus == "true";

            var result = await _purchaseService.FreeWithdrawStatus(id, status);

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