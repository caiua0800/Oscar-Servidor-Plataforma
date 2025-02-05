public class MonthData
{
    public string MonthName { get; set; } // Nome do mês (ex: "JAN", "FEV")
    public decimal MonthValue { get; set; } // Valor total das compras no mês
    public int PurchasesQtt { get; set; } // Quantidade de compras no mês
}

public class LastSixMonthsPurchaseData
{
    public int NumberOfMonths { get; set; } // Número de meses (sempre 6)
    public decimal PercentageChange { get; set; } // Porcentagem de mudança nas compras
    public List<MonthData> MonthsData { get; set; } // Dados mensais
}