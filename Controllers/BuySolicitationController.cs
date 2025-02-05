using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuySolicitationController : ControllerBase
    {
        private readonly BuySolicitationService _buySolicitationService;
        private readonly VendaService _vendaService;
        private readonly PurchaseService _purchaseService;
        private readonly ClientService _clientService;

        public BuySolicitationController(BuySolicitationService buySolicitationService, VendaService vendaService, PurchaseService purchaseService, ClientService clientService)
        {
            _buySolicitationService = buySolicitationService;
            _vendaService = vendaService;
            _purchaseService = purchaseService;
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BuySolicitation buySolicitation)
        {
            if (buySolicitation == null)
            {
                return BadRequest("Venda is null.");
            }

            var createdVenda = await _buySolicitationService.CreateBuySolicitationsAsync(buySolicitation);
            return CreatedAtAction(nameof(GetBuySolicitationById), new { id = createdVenda.Id }, createdVenda);
        }

        [HttpPost("pix/{vendaId}/{buyerId}")]
        public async Task<IActionResult> Create(string vendaId, string buyerId)
        {
            if (vendaId.Trim() == "" || vendaId == null || buyerId.Trim() == "" || buyerId == null)
            {
                return BadRequest("vendaId or buyerId null.");
            }

            var createdVenda = await _buySolicitationService.CreatePix(vendaId, buyerId);
            return Ok(createdVenda);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bs = await _buySolicitationService.GetAllBuySolicitationsAsync();
            return Ok(bs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuySolicitationById(string id)
        {
            var buySolicitation = await _buySolicitationService.GetBuySolicitationByIdAsync(id);
            if (buySolicitation == null)
            {
                return NotFound();
            }
            return Ok(buySolicitation);
        }

        [HttpGet("client/{id}")]
        public async Task<IActionResult> GetBuySolicitationByClientId(string id)
        {
            var bs = await _buySolicitationService.GetBuySolicitationByClientIdAsync(id);
            if (bs == null)
            {
                return NotFound();
            }
            return Ok(bs);
        }

        [HttpGet("venda/{id}")]
        public async Task<IActionResult> GetBuySolicitationByVendaId(string id)
        {
            var bs = await _buySolicitationService.GetBuySolicitationByVendaIdAsync(id);
            if (bs == null)
            {
                return NotFound();
            }
            return Ok(bs);
        }

        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> ChangeBuySolicitationStatus(string id)
        {
            var bs = await _buySolicitationService.GetBuySolicitationByIdAsync(id);
            if (bs == null)
            {
                return NotFound();
            }

            var buyerId = bs.ClientId;
            var vendaId = bs.VendaId;

            var venda = await _vendaService.GetVendaByIdAsync(vendaId);

            if (venda == null)
            {
                return NotFound("Venda não encontrada.");
            }

            Purchase? purchaseBeingSold = await _purchaseService.GetPurchaseByIdAsync(venda.PurchaseId);

            purchaseBeingSold.ClientId = buyerId;

            await _clientService.WithdrawFromBlockedBalanceAsync(venda.SellerId, purchaseBeingSold.TotalPrice);
            await _clientService.WithdrawFromBalanceAsync(venda.SellerId, purchaseBeingSold.TotalPrice + (purchaseBeingSold.CurrentIncome - purchaseBeingSold.AmountWithdrawn));
            await _clientService.AddToExtraBalanceAsync(venda.SellerId, venda.TotalPrice + (purchaseBeingSold.CurrentIncome - purchaseBeingSold.AmountWithdrawn));
            await _purchaseService.DeletePurchaseAsync(purchaseBeingSold.PurchaseId);
            await _vendaService.UpdateVendaStatusAndBuyerAsync(vendaId, 2, buyerId);
            await _buySolicitationService.UpdateBuySolicitationStatus(bs.Id, 2);

            purchaseBeingSold.AmountWithdrawn = purchaseBeingSold.CurrentIncome;
            purchaseBeingSold.FirstOwner = venda.SellerId;

            Purchase newPurchase = await _purchaseService.CreatePurchaseVendaAsync(purchaseBeingSold);
            return Ok(newPurchase);
        }
    }
}
