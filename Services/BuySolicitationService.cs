using System.Text;
using System.Text.Json;
using DotnetBackend.Models;
using MongoDB.Driver;

namespace DotnetBackend.Services;


public class BuySolicitationService
{
    private readonly IMongoCollection<BuySolicitation> _buySolicitations;
    private readonly CounterService _counterService;
    private readonly VendaService _vendaService;
    private readonly ClientService _clientService;

    public BuySolicitationService(MongoDbService mongoDbService, CounterService counterService, VendaService vendaService, ClientService clientService)
    {
        _buySolicitations = mongoDbService.GetCollection<BuySolicitation>("BuySolicitations");
        _counterService = counterService;
        _vendaService = vendaService;
        _clientService = clientService;
    }

    public async Task<BuySolicitation> CreateBuySolicitationsAsync(BuySolicitation buySolicitation)
    {
        buySolicitation.Id = "BS" + await _counterService.GetNextSequenceAsync("BuySolicitation");
        TimeZoneInfo brtZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        DateTime currentBrasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brtZone);
        buySolicitation.DateCreated = currentBrasiliaTime;
        await _buySolicitations.InsertOneAsync(buySolicitation);
        return buySolicitation;
    }

    // public async Task<BuySolicitation?> CreatePix(string vendaId, string buyerId)
    // {
    //     var venda = await _vendaService.GetVendaByIdAsync(vendaId);
    //     if (venda == null)
    //     {
    //         return null;
    //     }

    //     var client = await _clientService.GetClientByIdAsync(buyerId);
    //     if (client == null)
    //     {
    //         return null;
    //     }

    //     var ordemDeCompra = new BuySolicitation(client.Id, vendaId);

    //     ordemDeCompra.TotalPrice = venda.TotalPrice;
    //     ordemDeCompra.ContractPrice = venda.ContractPrice;
    //     ordemDeCompra.PurchaseId = venda.PurchaseId;
    //     ordemDeCompra.FinalIncome = venda.FinalIncome;

    //     using (var httpClient = new HttpClient())
    //     {
    //         var requestPayload = new
    //         {
    //             transaction_amount = venda.TotalPrice,
    //             description = $"Pagamento de Compra do contrato {venda.PurchaseId}",
    //             paymentMethodId = "pix",
    //             email = client.Email,
    //             identificationType = "CPF",
    //             number = client.Id
    //         };

    //         var jsonContent = JsonSerializer.Serialize(requestPayload);
    //         var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    //         HttpResponseMessage response = await httpClient.PostAsync("http://localhost:4040/pix", httpContent);

    //         if (response.IsSuccessStatusCode)
    //         {
    //             var responseBody = await response.Content.ReadAsStringAsync();
    //             var pixResponse = JsonSerializer.Deserialize<PixResponse>(responseBody);

    //             ordemDeCompra.TicketPayment = pixResponse?.PointOfInteraction.TransactionData.TicketUrl;
    //             ordemDeCompra.QrCode = pixResponse?.PointOfInteraction.TransactionData.QrCode;
    //             ordemDeCompra.TicketId = pixResponse?.Id?.ToString();
    //             ordemDeCompra.ExpirationDate = pixResponse?.PointOfInteraction.TransactionData.ExpirationDate;
    //             ordemDeCompra.Id = "BS" + await _counterService.GetNextSequenceAsync("BuySolicitation");
    //             TimeZoneInfo brtZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
    //             DateTime currentBrasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brtZone);
    //             ordemDeCompra.DateCreated = currentBrasiliaTime;

    //         }
    //         else
    //         {
    //             ordemDeCompra.TicketPayment = null;
    //             ordemDeCompra.QrCode = null;
    //             ordemDeCompra.TicketId = null;
    //             ordemDeCompra.ExpirationDate = null;
    //         }
    //         await _buySolicitations.InsertOneAsync(ordemDeCompra);

    //     }

    //     return ordemDeCompra;
    // }

    public async Task<BuySolicitation?> CreatePix(string vendaId, string buyerId)
    {
        try
        {
            var venda = await _vendaService.GetVendaByIdAsync(vendaId);
            if (venda == null)
            {
                return null;
            }

            var client = await _clientService.GetClientByIdAsync(buyerId);
            if (client == null)
            {
                return null;
            }

            var ordemDeCompra = new BuySolicitation(client.Id, vendaId);

            ordemDeCompra.TotalPrice = venda.TotalPrice;
            ordemDeCompra.ContractPrice = venda.ContractPrice;
            ordemDeCompra.PurchaseId = venda.PurchaseId;
            ordemDeCompra.FinalIncome = venda.FinalIncome;
            ordemDeCompra.Id = "BS" + await _counterService.GetNextSequenceAsync("BuySolicitation");
            
            using (var httpClient = new HttpClient())
            {
                var requestPayload = new
                {
                    transaction_amount = venda.TotalPrice,
                    description = $"Pagamento de Compra do contrato {venda.PurchaseId}",
                    paymentMethodId = "pix",
                    email = client.Email,
                    identificationType = "CPF",
                    number = client.Id
                };

                var jsonContent = JsonSerializer.Serialize(requestPayload);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:4040/pix", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var pixResponse = JsonSerializer.Deserialize<PixResponse>(responseBody);

                    ordemDeCompra.TicketPayment = pixResponse?.PointOfInteraction.TransactionData.TicketUrl;
                    ordemDeCompra.QrCode = pixResponse?.PointOfInteraction.TransactionData.QrCode;
                    ordemDeCompra.TicketId = pixResponse?.Id?.ToString();
                    ordemDeCompra.ExpirationDate = pixResponse?.PointOfInteraction.TransactionData.ExpirationDate;
                    ordemDeCompra.Id = "BS" + await _counterService.GetNextSequenceAsync("BuySolicitation");
                    TimeZoneInfo brtZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                    DateTime currentBrasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brtZone);
                    ordemDeCompra.DateCreated = currentBrasiliaTime;
                }
                else
                {
                    ordemDeCompra.TicketPayment = null;
                    ordemDeCompra.QrCode = null;
                    ordemDeCompra.TicketId = null;
                    ordemDeCompra.ExpirationDate = null;
                }

                await _buySolicitations.InsertOneAsync(ordemDeCompra);
            }

            return ordemDeCompra;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gerar PIX: {ex.Message}");
            return null; 
        }
    }

    public async Task<List<BuySolicitation>> GetAllBuySolicitationsAsync()
    {
        return await _buySolicitations.Find(_ => true).ToListAsync();
    }

    public async Task<List<BuySolicitation>> GetBuySolicitationByClientIdAsync(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID must be provided.", nameof(clientId));
        }

        return await _buySolicitations.Find(v => v.ClientId == clientId).ToListAsync();
    }

    public async Task<List<BuySolicitation>> GetBuySolicitationByVendaIdAsync(string vendaId)
    {
        if (string.IsNullOrWhiteSpace(vendaId))
        {
            throw new ArgumentException("Venda ID must be provided.", nameof(vendaId));
        }

        return await _buySolicitations.Find(v => v.VendaId == vendaId).ToListAsync();
    }

    public async Task<BuySolicitation?> GetBuySolicitationByIdAsync(string id)
    {
        var normalizedId = id.Trim();
        return await _buySolicitations.Find(p => p.Id == normalizedId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateBuySolicitationStatus(string id, int newStatus)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Buy ID must be provided.", nameof(id));
        }

        if (newStatus < 1 || newStatus > 3)
        {
            throw new ArgumentException("New status must be between 1 and 3, and buy solicitation must be provided.", nameof(newStatus));
        }

        var filter = Builders<BuySolicitation>.Filter.Eq("Id", id);
        var update = Builders<BuySolicitation>.Update
                        .Set("Status", newStatus);

        var updateResult = await _buySolicitations.UpdateOneAsync(filter, update);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }
}