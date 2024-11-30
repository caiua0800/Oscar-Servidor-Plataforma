using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DotnetBackend.Models
{
    public class Purchase
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? PurchaseId { get; set; }

        [BsonElement("clientId")]
        public required string ClientId { get; set; }

        [BsonElement("productName")]
        public string? ProductName { get; set; }

        [BsonElement("quantity")]
        public required int Quantity { get; set; }

        [BsonElement("unityPrice")]
        public decimal UnityPrice { get; set; }

        [BsonElement("discount")]
        public required decimal Discount { get; set; }

        [BsonElement("purchaseDate")]
        public DateTime? PurchaseDate { get; set; }

        [BsonElement("totalPrice")]
        public decimal TotalPrice { get; set; }

        [BsonElement("amountPaid")]
        public decimal AmountPaid { get; set; }

        [BsonElement("currentIncome")]
        public decimal CurrentIncome { get; set; } = 0;

        [BsonElement("amountWithdrawn")] // Use este nome
        public decimal AmountWithdrawn { get; set; } = 0; // Corrigido aqui

        [BsonElement("percentageProfit")]
        public double PercentageProfit { get; set; } = 0;

        [BsonElement("finalIncome")]
        public decimal FinalIncome { get; set; }

        [BsonElement("coin")]
        public required string Coin { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("type")]
        public int Type { get; set; }

        [BsonElement("endContractDate")]
        public DateTime? EndContractDate { get; set; }

        [BsonElement("lastIncreasement")]
        public DateTime? LastIncreasement { get; set; }

        [BsonElement("firstIncreasement")]
        public DateTime? FirstIncreasement { get; set; }

        [BsonElement("description")]
        public string Description { get; set; } = "";

        [BsonElement("ticketPayment")]
        public string? TicketPayment { get; set; } = "";

        [BsonElement("ticketId")]
        public string? TicketId { get; set; } = "";

        [BsonElement("qrCode")]
        public string? QrCode { get; set; } = "";
        
        [BsonElement("qrCodeBase64")]
        public string? QrCodeBase64 { get; set; } = "";

        public Purchase() { }

        public Purchase(string clientId, int quantity, decimal unityPrice, decimal discount, string coin, int type)
        {
            ClientId = clientId;
            Quantity = quantity;
            UnityPrice = unityPrice;
            Discount = discount;
            AmountWithdrawn = 0;
            Coin = coin;
            Status = 1;
            PurchaseDate = DateTime.UtcNow;
            AmountPaid = CalculateTotalPrice(quantity, unityPrice, discount);
            TotalPrice = unityPrice * quantity;
        }

        private decimal CalculateTotalPrice(int quantity, decimal unityPrice, decimal discount)
        {
            return (quantity * unityPrice) - ((quantity * unityPrice) * discount);
        }

        public void WithdrawFromPurchase(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser sacado deve ser positivo.");
            }
            if (amount > (CurrentIncome - AmountWithdrawn))
            {
                throw new InvalidOperationException("Saldo indisponível.");
            }
            AmountWithdrawn += amount;
        }
    }
}