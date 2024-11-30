using MongoDB.Driver;
using DotnetBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DotnetBackend.Services;

public class WithdrawalService
{
    private readonly IMongoCollection<Withdrawal> _withdrawals;
    private readonly CounterService _counterService;
    private readonly ExtractService _extractService;
    private readonly ClientService _clientService;
    private readonly PurchaseService _purchaseService;
    public WithdrawalService(MongoDbService mongoDbService, ClientService clientService, CounterService counterService, ExtractService extractService, PurchaseService purchaseService)
    {
        _withdrawals = mongoDbService.GetCollection<Withdrawal>("Withdrawal");
        _counterService = counterService;
        _extractService = extractService;
        _clientService = clientService;
        _purchaseService = purchaseService;
    }

    public async Task<Withdrawal> CreateWithdrawalAsync(Withdrawal withdrawal)
    {
        if (withdrawal.DateCreated == null)
        {
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateTime brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, brasiliaTimeZone);
            withdrawal.DateCreated = brasiliaTime;
        }

        if (withdrawal.ItemId == null)
        {
            withdrawal.ItemId = "Retirado";
        }
        var client = await _clientService.GetClientByIdAsync(withdrawal.ClientId);
        if (client == null)
        {
            throw new InvalidOperationException("Cliente não encontrado.");
        }

        try
        {
            await _clientService.WithdrawFromBalanceAsync(withdrawal.ClientId, (decimal)withdrawal.AmountWithdrawn);
            await _purchaseService.WithdrawFromPurchaseAsync(withdrawal.ItemId, (decimal)withdrawal.AmountWithdrawn);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Erro ao tentar sacar: {ex.Message}");
            throw new InvalidOperationException("Não foi possível realizar o saque: " + ex.Message);
        }

        withdrawal.WithdrawalId = "W" + await _counterService.GetNextSequenceAsync("withdrawal");
        withdrawal.AmountWithdrawn = withdrawal.AmountWithdrawn;

        await _withdrawals.InsertOneAsync(withdrawal);
        var extract = new Extract($"Saque do contrato {withdrawal.ItemId}", (decimal)withdrawal.AmountWithdrawn, withdrawal.ClientId);
        await _extractService.CreateExtractAsync(extract);
        await _clientService.AddWithdrawalAsync(withdrawal.ClientId, withdrawal.WithdrawalId);

        return withdrawal;
    }

    public async Task<Withdrawal> CreateWithdrawalExtraBalanceAsync(Withdrawal withdrawal)
    {
        if (withdrawal.DateCreated == null)
        {
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateTime brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, brasiliaTimeZone);
            withdrawal.DateCreated = brasiliaTime;
        }

        if (withdrawal.ItemId == null)
        {
            withdrawal.ItemId = "Retirado";
        }
        var client = await _clientService.GetClientByIdAsync(withdrawal.ClientId);
        if (client == null)
        {
            throw new InvalidOperationException("Cliente não encontrado.");
        }

        try
        {
            await _clientService.WithdrawFromBalanceAsync(withdrawal.ClientId, (decimal)withdrawal.AmountWithdrawn);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Erro ao tentar sacar: {ex.Message}");
            throw new InvalidOperationException("Não foi possível realizar o saque: " + ex.Message);
        }

        withdrawal.WithdrawalId = "W" + await _counterService.GetNextSequenceAsync("withdrawal");
        withdrawal.AmountWithdrawn = withdrawal.AmountWithdrawn;

        await _withdrawals.InsertOneAsync(withdrawal);
        var extract = new Extract($"Saque da Carteira", (decimal)withdrawal.AmountWithdrawn, withdrawal.ClientId);
        await _extractService.CreateExtractAsync(extract);
        await _clientService.AddWithdrawalAsync(withdrawal.ClientId, withdrawal.WithdrawalId);

        return withdrawal;
    }

    public async Task<Withdrawal?> GetWithdrawalByIdAsync(string id)
    {
        var normalizedId = id.Trim();
        return await _withdrawals.Find(p => p.WithdrawalId == normalizedId).FirstOrDefaultAsync();
    }

    public async Task<List<Withdrawal>> GetAllWithdrawalsAsync()
    {
        return await _withdrawals.Find(_ => true).ToListAsync(); // Retorna todos os saques
    }

    public async Task<List<Withdrawal>> GetAllWithdrawsToPay()
    {
        return await _withdrawals.Find(w => w.Status == 1).ToListAsync();
    }

    public async Task<bool> DeleteWithdrawalAsync(string id)
    {
        var normalizedId = id.Trim();
        var saqueEncontrado = await _withdrawals.Find(p => p.WithdrawalId == normalizedId).FirstOrDefaultAsync();
        Console.WriteLine($"saqueEncontrado: {saqueEncontrado}");

        if (saqueEncontrado == null)
        {
            Console.WriteLine($"Withdrawal with ID {normalizedId} not found.");
            return false; // Saque não encontrado, retorna falso
        }

        var deleteResult = await _withdrawals.DeleteOneAsync(w => w.WithdrawalId == normalizedId);
        Console.WriteLine($"deleteResult: {deleteResult}");
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }

    public async Task<bool> UpdateStatus(string withdrawalId, int newStatus)
    {
        var existingWithdrawal = await GetWithdrawalByIdAsync(withdrawalId);

        if (existingWithdrawal != null)
        {
            existingWithdrawal.Status = newStatus;
            var updateDefinition = Builders<Withdrawal>.Update.Set(w => w.Status, newStatus);
            await _withdrawals.UpdateOneAsync(w => w.WithdrawalId == withdrawalId, updateDefinition);
            Console.WriteLine($"Saque encontrado: {existingWithdrawal.WithdrawalId}, novo status {newStatus}");

            if (newStatus == 3)
            {
                Console.WriteLine("Cancelando");
                await _purchaseService.RemoveSomeAmountWithdrawn(existingWithdrawal.ItemId, (decimal)(existingWithdrawal.AmountWithdrawn)); //Coloquei dividido por 2 pq algum bug satânico tava de algum jeito dobrando esse valor não sei Jesus como
                var extract = new Extract($"Devolução do Saque Negado referente a #{existingWithdrawal.ItemId}", (decimal)existingWithdrawal.AmountWithdrawn, existingWithdrawal.ClientId);
                await _extractService.CreateExtractAsync(extract);
                Console.WriteLine($"Valor de {existingWithdrawal.AmountWithdrawn} adicionado ao saldo do cliente {existingWithdrawal.ClientId}");
            }
            return true;
        }
        else
        {
            Console.WriteLine($"Erro ao encontrar saque {withdrawalId}");
            return false;
        }
    }


}