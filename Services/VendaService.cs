using DotnetBackend.Models;
using MongoDB.Driver;

namespace DotnetBackend.Services;


public class VendaService
{
    private readonly IMongoCollection<Venda> _vendas;
    private readonly CounterService _counterService;

    public VendaService(MongoDbService mongoDbService, CounterService counterService)
    {
        _vendas = mongoDbService.GetCollection<Venda>("vendas");
        _counterService = counterService;
    }

    public async Task<Venda> CreateVendaAsync(Venda venda)
    {
        venda.Id = "V" + await _counterService.GetNextSequenceAsync("vendas");
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

        return await _vendas.Find(v => v.FromWho == clientId).ToListAsync(); 
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

    public async Task<Venda?> GetVendaByClientIdAsync(string id)
    {
        var normalizedId = id.Trim();
        return await _vendas.Find(p => p.FromWho == normalizedId).FirstOrDefaultAsync();
    }


}