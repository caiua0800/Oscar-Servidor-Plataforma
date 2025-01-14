using MongoDB.Driver;
using DotnetBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetBackend.Services;

public class IndicationService
{
    private readonly IMongoCollection<Indication> _indications;
    private readonly CounterService _counterService;
    private readonly ExtractService _extractService;

    public readonly ClientService _clientService;


    public IndicationService(MongoDbService mongoDbService, CounterService counterService, ExtractService extractService, ClientService clientService)
    {
        _indications = mongoDbService.GetCollection<Indication>("Indications");
        _counterService = counterService;
        _extractService = extractService;
        _clientService = clientService;
    }

    public async Task<Indication> CreateIndicationAsync(Indication indication)
    {
        int nextSequence = await _counterService.GetNextSequenceAsync("Indication");
        indication.Id = "I" + nextSequence;
        TimeZoneInfo brtZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        DateTime currentBrasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brtZone);
        indication.DateCreated = currentBrasiliaTime;
        // var extract = new Extract($"Indicação de R${indication.Value} adicionado ao cliente {indication.ClientId}", (decimal)indication.Value, indication.ClientId);
        // await _extractService.CreateExtractAsync(extract);
        await _indications.InsertOneAsync(indication);
        return indication;
    }

    public async Task<bool> DeleteIndicationAsync(string id)
    {
        var indication = await GetIndicationByIdAsync(id);
        if (indication == null)
        {
            return false;
        }

        await _clientService.WithdrawFromExtraBalanceAsync(indication.ClientId, (decimal)indication.Value);

        var deleteResult = await _indications.DeleteOneAsync(c => c.Id == id);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }

    public async Task<Indication?> GetIndicationByIdAsync(string id)
    {
        return await _indications.Find(c => c.Id == id).FirstOrDefaultAsync();
    }
    public async Task<List<Indication>> GetIndicationsByClientIdAsync(string id)
    {
        return await _indications.Find(c => c.ClientId == id).ToListAsync();
    }

    public async Task<List<Indication>> GetAllIndicationsAsync()
    {
        return await _indications.Find(_ => true).ToListAsync();
    }
}

