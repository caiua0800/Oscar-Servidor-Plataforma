using MongoDB.Driver;
using DotnetBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DotnetBackend.Services;

public class GatewayService
{
    private readonly WithdrawalService _withdrawalService;
    private readonly ClientService _clientService;
    private readonly PurchaseService _purchaseService;
    private readonly BankAccountService _bankAccountService;
    private readonly AdminWithdrawalService _adminWithdrawalService;
    public GatewayService(ClientService clientService, PurchaseService purchaseService, WithdrawalService withdrawalService, BankAccountService bankAccountService, AdminWithdrawalService adminWithdrawalService)
    {
        _clientService = clientService;
        _purchaseService = purchaseService;
        _withdrawalService = withdrawalService;
        _bankAccountService = bankAccountService;
        _adminWithdrawalService = adminWithdrawalService;
    }

    public async Task<PlatformInfo> GetPlatformInfos()
    {
        List<Client> all_clients = await _clientService.GetAllClientsAsync();
        List<Withdrawal> all_withdrawals = await _withdrawalService.GetAllWithdrawalsAsync();
        List<Purchase> all_purchases = await _purchaseService.GetAllPurchasesAsync();
        List<AdminWithdrawal> all_admin_withdrawals = await _adminWithdrawalService.GetAllAdminWithdrawsToPay();

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        int purchasesThisMonth = all_purchases.Count(p =>
            p.PurchaseDate.HasValue &&
            p.PurchaseDate.Value.Month == currentMonth &&
            p.PurchaseDate.Value.Year == currentYear);

        decimal totalAmountPurchasesThisMonth = all_purchases
            .Where(p =>
                p.PurchaseDate.HasValue &&
                p.PurchaseDate.Value.Month == currentMonth &&
                p.PurchaseDate.Value.Year == currentYear)
            .Sum(p => p.AmountPaid); // Somando AmountPaid

        int total_purchases_active = all_purchases.Count(p => p.Status == 2);

        decimal totalAmountActivePurchases = all_purchases
            .Where(p => p.Status == 2)
            .Sum(p => p.AmountPaid);

        int total_withdrawals = all_withdrawals.Count(p =>
            p.Status.HasValue &&
            p.Status == 1);

        double totalAmountToWithdraw = all_withdrawals
            .Where(w => w.Status == 1)
            .Sum(w => w.AmountWithdrawn);

        double? totalAdminAmountToWithdrawal = all_admin_withdrawals
            .Where(w => w.Status == 1)
            .Sum(w => w.AmountWithdrawnToPay);

        BankAccount bankAccount = await _bankAccountService.GetBankAccountAsync();

        var platformInfo = new PlatformInfo
        {
            TotalClients = all_clients.Count,
            TotalWithdrawals = total_withdrawals,
            TotalPurchasesActive = total_purchases_active,
            PurchasesThisMonth = purchasesThisMonth,
            TotalAmountToWithdraw = totalAmountToWithdraw,
            TotalAmountActivePurchases = totalAmountActivePurchases,
            TotalAmountPurchasesThisMonth = totalAmountPurchasesThisMonth,
            BankAccountValue = bankAccount.Balance,
            TotalAdminAmountToWithdraw = (double)totalAdminAmountToWithdrawal
        };

        return platformInfo;
    }

    public async Task<List<Withdrawal>> GetAllWithdrawals()
    {
        try
        {
            return await _withdrawalService.GetAllWithdrawsToPay();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter saques.", ex);
        }
    }

    public async Task<List<AdminWithdrawal>> GetAllAdminWithdrawals()
    {
        try
        {
            return await _adminWithdrawalService.GetAllAdminWithdrawsToPay();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter saques.", ex);
        }
    }


}