using MongoDB.Driver;
using DotnetBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DotnetBackend.Services;

public class AdminWithdrawalService
{
    private readonly IMongoCollection<AdminWithdrawal> _adminWithdrawals;
    private readonly CounterService _counterService;
    private readonly ExtractService _extractService;
    private readonly BankAccountService _bankAccountService;
    public AdminWithdrawalService(MongoDbService mongoDbService, 
    CounterService counterService, ExtractService extractService, BankAccountService bankAccountService)
    {
        _adminWithdrawals = mongoDbService.GetCollection<AdminWithdrawal>("AdminWithdrawals");
        _counterService = counterService;
        _extractService = extractService;
        _bankAccountService = bankAccountService;
    }

    public async Task<AdminWithdrawal> CreateWithdrawalAsync(AdminWithdrawal adminWithdrawal)
    {
        if (adminWithdrawal.DateCreated == null)
        {
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateTime brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, brasiliaTimeZone);
            adminWithdrawal.DateCreated = brasiliaTime;
        }

        adminWithdrawal.AdminWithdrawalId = "W" + await _counterService.GetNextSequenceAsync("withdrawal");
        adminWithdrawal.AmountWithdrawn = adminWithdrawal.AmountWithdrawn;
        adminWithdrawal.AmountWithdrawnToPay = adminWithdrawal.AmountWithdrawn * 0.975;

        await _adminWithdrawals.InsertOneAsync(adminWithdrawal);
        var extract = new Extract($"Solicitação de Saque do Admin", (decimal)adminWithdrawal.AmountWithdrawn, "Admin");
        await _extractService.CreateExtractAsync(extract);

        return adminWithdrawal;
    }

    public async Task<AdminWithdrawal?> GetAdminWithdrawalByIdAsync(string id)
    {
        var normalizedId = id.Trim();
        return await _adminWithdrawals.Find(p => p.AdminWithdrawalId == normalizedId).FirstOrDefaultAsync();
    }

    public async Task<List<AdminWithdrawal>> GetAllAdminWithdrawalsAsync()
    {
        return await _adminWithdrawals.Find(_ => true).ToListAsync(); // Retorna todos os saques
    }

    public async Task<List<AdminWithdrawal>> GetAllAdminWithdrawsToPay()
    {
        return await _adminWithdrawals.Find(w => w.Status == 1).ToListAsync();
    }

    public async Task<bool> UpdateStatus(string adminWithdrawalId, int newStatus)
    {
        var existingAdminWithdrawal = await GetAdminWithdrawalByIdAsync(adminWithdrawalId);

        if (existingAdminWithdrawal != null)
        {
            existingAdminWithdrawal.Status = newStatus;
            var updateDefinition = Builders<AdminWithdrawal>.Update.Set(w => w.Status, newStatus);
            await _adminWithdrawals.UpdateOneAsync(w => w.AdminWithdrawalId == adminWithdrawalId, updateDefinition);
            await _bankAccountService.WithdrawFromBalanceAsync(adminWithdrawalId, (decimal)existingAdminWithdrawal.AmountWithdrawn);
            Console.WriteLine($"Saque encontrado: {existingAdminWithdrawal.AdminWithdrawalId}, novo status {newStatus}");

            return true;
        }
        else
        {
            Console.WriteLine($"Erro ao encontrar saque {adminWithdrawalId}");
            return false;
        }
    }


}