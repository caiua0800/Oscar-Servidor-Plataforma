namespace DotnetBackend.Models
{
    public class HighestSpenderResult
    {
        public Client Client { get; set; } // Dados do cliente
        public decimal TotalSpent { get; set; } // Total gasto pelo cliente
    }
}
