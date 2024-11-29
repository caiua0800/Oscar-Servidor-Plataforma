using MongoDB.Driver;
using DotnetBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetBackend.Services;

namespace DotnetBackend.Queries
{
    public class ClientQueries
    {
        private readonly IMongoCollection<Client> _clients;

        public ClientQueries(MongoDbService mongoDbService)
        {
            _clients = mongoDbService.GetCollection<Client>("Clients");
        }

        // Método para pegar todos os clientes em ordem alfabética
        public async Task<List<Client>> GetAllClientsSortedAsync()
        {
            var clients = await _clients.Find(_ => true).ToListAsync();
            return clients.OrderBy(c => c.Name).ToList(); // Ordena por nome
        }

    //     public async Task<HighestSpenderResult?> GetClientWithHighestSpendingAsync(PurchaseService purchaseService)
    //     {
    //         // Obtém todas as compras
    //         var purchases = await purchaseService.GetAllPurchasesAsync();

    //         // Agrupa as compras pelo clientId e calcula o total gasto
    //         var clientSpending = purchases
    //             .GroupBy(p => p.ClientId)
    //             .Select(g => new
    //             {
    //                 ClientId = g.Key,
    //                 TotalSpent = g.Sum(p => p.TotalPrice.GetValueOrDefault())

    //             })
    //             .OrderByDescending(x => x.TotalSpent)
    //             .FirstOrDefault(); // Pega o cliente que mais gastou

    //         // Se não houver compras, retorna null
    //         if (clientSpending == null)
    //             return null;

    //         // Busca o cliente correspondente ao clientId
    //         var client = await _clients.Find(c => c.Id == clientSpending.ClientId).FirstOrDefaultAsync();

    //         // Retorna o resultado com o cliente e o total gasto
    //         return new HighestSpenderResult
    //         {
    //             Client = client,
    //             TotalSpent = clientSpending.TotalSpent
    //         };
    //     }

    }
}
