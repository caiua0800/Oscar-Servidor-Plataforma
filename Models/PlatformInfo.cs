using DotnetBackend.Models;
using System;

public class MonthModel
{
    public string Month { get; set; }
    public double Value { get; set; }
}

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
    public double? TotalAmountWithdrawn { get; set; }
    public double? TotalAmountWithdrawnClients { get; set; }
    public double? TotalAmountWithdrawnAdmin { get; set; }
    public MonthModel[] TotalWithdrawalsLastFourMonths { get; set; }
}