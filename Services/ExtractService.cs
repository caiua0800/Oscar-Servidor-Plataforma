// ExtractService.cs
using DotnetBackend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetBackend.Services
{
    public class ExtractService
    {
        private readonly IMongoCollection<Extract> _extracts;
        private readonly CounterService _counterService;

        public ExtractService(MongoDbService mongoDbService, CounterService counterService)
        {
            _extracts = mongoDbService.GetCollection<Extract>("Extracts");
            _counterService = counterService;
        }

        public async Task<Extract> CreateExtractAsync(Extract extract)
        {
            extract.ExtractId = "E" + await _counterService.GetNextSequenceAsync("extracts");

            await _extracts.InsertOneAsync(extract);
            return extract;
        }


        public async Task<List<Extract>> GetAllExtractsAsync()
        {
            // Retorna todos os extratos
            return await _extracts.Find(_ => true).ToListAsync();
        }

        public async Task<Extract?> GetExtractByIdAsync(string id)
        {
            // Remove espaços em branco do ID informado
            var normalizedId = id.Trim();
            return await _extracts.Find(e => e.ExtractId == normalizedId).FirstOrDefaultAsync();
        }

        public async Task<List<Extract>> GetLastExtractsByClientIdAsync(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("Client ID must be provided.", nameof(clientId));
            }

            return await _extracts
                .Find(e => e.ClientId == clientId)
                .SortByDescending(e => e.ExtractId)
                .Limit(50)
                .ToListAsync();
        }

        public async Task<List<Extract>> GetLast50ExtractsAsync()
        {
            // Obtém os últimos 50 extratos, ordenados pelo ExtractId
            return await _extracts
                .Find(_ => true) // Pega todos os extratos
                .SortByDescending(e => e.ExtractId)
                .Limit(50)
                .ToListAsync();
        }



        public async Task<List<Extract>> GetExtractsByClientIdAsync(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("Client ID must be provided.", nameof(clientId));
            }

            return await _extracts.Find(e => e.ClientId == clientId).ToListAsync(); // Retorna extratos pelo ClientId
        }

        public async Task<bool> DeleteExtractAsync(string id)
        {
            var deleteResult = await _extracts.DeleteOneAsync(e => e.ExtractId == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0; // Retorna true se a remoção foi bem-sucedida
        }

        public async Task<List<Extract>> GetExtractsContainingStringAsync(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                throw new ArgumentException("A string de busca não pode ser vazia.", nameof(searchString));
            }
            string lowerSearchString = searchString.ToLower();

            return await _extracts.Find(e => e.Name.ToLower().Contains(lowerSearchString))
                .ToListAsync();
        }
    }
}
