using DotnetBackend.Models;

public class PlatformInfo
{
    public int TotalClients { get; set; }
    public int TotalWithdrawals { get; set; }
    public int TotalPurchases { get; set; }
    public int PurchasesThisMonth { get; set; }
    public int TotalPurchasesActive { get; set; }
    public double TotalAmountToWithdraw { get; set; }
    public double TotalAdminAmountToWithdraw { get; set; }
    public decimal TotalAmountActivePurchases { get; set; }
    public decimal TotalAmountPurchasesThisMonth { get; set; }
    public decimal? BankAccountValue { get; set; }

}