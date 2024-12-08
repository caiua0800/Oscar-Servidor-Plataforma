using Microsoft.AspNetCore.Mvc;
using DotnetBackend.Models;
using DotnetBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DotnetBackend.Queries;
using System.Collections.Generic;

namespace DotnetBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ClientService _clientService;
        private readonly ClientQueries _clientQueries;
        private readonly PurchaseService _purchaseService;
        private readonly ExtractService _extractService;
        private readonly WithdrawalService _withdrawalService;
        private readonly VendaService _vendaService;
        private readonly WebSocketHandler _webSocketHandler;

        public ClientController(ClientService clientService, ClientQueries clientQueries,
            PurchaseService purchaseService, ExtractService extractService,
            VendaService vendaService, WithdrawalService withdrawalService, WebSocketHandler webSocketHandler)
        {
            _clientService = clientService;
            _clientQueries = clientQueries;
            _purchaseService = purchaseService;
            _extractService = extractService;
            _vendaService = vendaService;
            _webSocketHandler = webSocketHandler;
            _withdrawalService = withdrawalService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            Console.WriteLine(client.Name);

            if (client == null || string.IsNullOrEmpty(client.Password))
            {
                return BadRequest("Client ou a senha está nula.");
            }

            var createdClient = await _clientService.CreateClientAsync(client, client.Password);

            Console.WriteLine($"Cliente criado: {createdClient.Id}, {createdClient.Name}");

            await _webSocketHandler.SendNewClientAsync(createdClient);

            return CreatedAtAction(nameof(Create), new { id = createdClient.Id }, createdClient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(string id)
        {
            Console.WriteLine($"Buscando cliente com CPF: '{id}'");
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                Console.WriteLine($"Cliente com CPF '{id}' não encontrado.");
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Client updatedClient)
        {
            if (updatedClient == null)
            {
                return BadRequest("O cliente atualizado não pode ser nulo.");
            }

            var result = await _clientService.UpdateClientAsync(id, updatedClient);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetClientDetails(string id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            var purchaseDetails = new List<Purchase>();
            decimal amountAvailableToWithdraw = 0;
            decimal amountTotalAvaliableFromContracts = 0;

            if (client.WalletExtract.Purchases != null)
            {
                foreach (var purchaseId in client.WalletExtract.Purchases)
                {
                    var purchase = await _purchaseService.GetPurchaseByIdAsync(purchaseId);
                    if (purchase != null)
                    {
                        purchaseDetails.Add(purchase);

                        if (purchase.FirstIncreasement.HasValue &&
                            (DateTime.UtcNow - purchase.FirstIncreasement.Value).TotalDays >= 90)
                        {
                            amountAvailableToWithdraw += (purchase.CurrentIncome - purchase.AmountWithdrawn);
                        }
                        else if (purchase.Status == 1 || purchase.Status == 2)
                        {
                            amountTotalAvaliableFromContracts += (purchase.CurrentIncome - purchase.AmountWithdrawn);
                        }
                    }
                }
            }


            amountAvailableToWithdraw += (decimal)client.ExtraBalance;

            var withdrawalDetails = new List<Withdrawal>();
            if (client.WalletExtract.Withdrawals != null)
            {
                foreach (var withdrawalId in client.WalletExtract.Withdrawals)
                {
                    var withdrawal = await _withdrawalService.GetWithdrawalByIdAsync(withdrawalId);
                    if (withdrawal != null)
                    {
                        withdrawalDetails.Add(withdrawal);
                    }
                }
            }

            var clientDetails = new
            {
                Client = new
                {
                    client.Id,
                    client.Name,
                    client.Email,
                    client.Phone,
                    client.Address,
                    client.Balance,
                    client.BlockedBalance,
                    client.ClientProfit,
                    client.DateCreated,
                    client.PlatformId,
                    client.WithdrawDate,
                    client.SponsorId,
                    client.Status,
                    AmountAvailableToWithdraw = amountAvailableToWithdraw,
                    WalletExtract = new
                    {
                        Purchases = purchaseDetails,
                        Withdrawals = withdrawalDetails
                    }
                }
            };

            return Ok(clientDetails);
        }

        [HttpPost("{id}/purchases")]
        public async Task<IActionResult> AddPurchase(string id, [FromBody] string purchaseId)
        {
            if (string.IsNullOrEmpty(purchaseId))
            {
                return BadRequest("ID da compra não pode ser nulo ou vazio.");
            }

            var result = await _clientService.AddPurchaseAsync(id, purchaseId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/uploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Arquivo não pode estar vazio.");
            }

            try
            {
                var imageUrl = await _clientService.UploadProfilePictureAsync(file);

                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    return NotFound();
                }

                client.ProfilePictureUrl = imageUrl;

                await _clientService.UpdateClientAsync(id, client);

                return Ok(new { Url = imageUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fazer upload da imagem: {ex.Message}");
                return StatusCode(500, "Erro ao fazer upload da imagem. Detalhes: " + ex.ToString());
            }
        }

        [HttpPut("{id}/exclude")]
        public async Task<IActionResult> ExcludeAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID do cliente inexistente.");
            }

            var purchases = await _purchaseService.GetPurchasesByClientIdAsync(id);

            if (purchases == null || !purchases.Any())
            {
                return NotFound("Nenhuma compra encontrada para este cliente.");
            }

            foreach (var purchase in purchases)
            {
                purchase.Status = 4;
                await _purchaseService.UpdatePurchaseAsync(purchase.PurchaseId, purchase);
            }

            // Exclui a conta do cliente
            var result = await _clientService.ExcludeAccount(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
