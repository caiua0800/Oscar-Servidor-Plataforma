using DotnetBackend.Models;
using MongoDB.Driver;

namespace DotnetBackend.Services;


public class VendaService
{
    private readonly IMongoCollection<Venda> _vendas;
    private readonly CounterService _counterService;
    private readonly PurchaseService _purchaseService;

    public VendaService(MongoDbService mongoDbService, CounterService counterService, PurchaseService purchaseService)
    {
        _vendas = mongoDbService.GetCollection<Venda>("Vendas");
        _counterService = counterService;
        _purchaseService = purchaseService;
    }

    public async Task<Venda> CreateVendaAsync(Venda venda)
    {
        venda.Id = "V" + await _counterService.GetNextSequenceAsync("vendas");
        TimeZoneInfo brtZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        DateTime currentBrasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brtZone);
        venda.DateCreated = currentBrasiliaTime;

        Purchase purchase = await _purchaseService.GetPurchaseByIdAsync(venda.PurchaseId);

        if (purchase == null)
        {
            return null;
        }

        venda.EndContractDate = purchase.EndContractDate;
        venda.FinalIncome = purchase.FinalIncome - purchase.CurrentIncome;

        await _vendas.InsertOneAsync(venda);
        return venda;
    }

    public async Task<List<Venda>> GetAllVendasAsync()
    {
        return await _vendas.Find(_ => true).ToListAsync();
    }

    public async Task<List<Venda>> GetVendasByClientIdAsync(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID must be provided.", nameof(clientId));
        }

        return await _vendas.Find(v => v.SellerId == clientId).ToListAsync();
    }

    public async Task<bool> DeleteVendaAsync(string id)
    {
        var deleteResult = await _vendas.DeleteOneAsync(p => p.Id == id);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }

    public async Task<Venda?> GetVendaByIdAsync(string id)
    {
        var normalizedId = id.Trim();
        return await _vendas.Find(p => p.Id == normalizedId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateVendaStatusAndBuyerAsync(string id, int newStatus, string newBuyer)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Venda ID must be provided.", nameof(id));
        }

        if (newStatus < 1 || newStatus > 3 || string.IsNullOrWhiteSpace(newBuyer))
        {
            throw new ArgumentException("New status must be between 1 and 3, and new buyer must be provided.", nameof(newStatus));
        }

        var filter = Builders<Venda>.Filter.Eq("Id", id);
        var update = Builders<Venda>.Update
                        .Set("Status", newStatus)
                        .Set("BuyerId", newBuyer);

        var updateResult = await _vendas.UpdateOneAsync(filter, update);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }
}