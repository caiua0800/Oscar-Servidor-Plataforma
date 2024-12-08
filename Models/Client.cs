using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DotnetBackend.Models
{
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("email")]
        public required string Email { get; set; }

        [BsonElement("phone")]
        public required string Phone { get; set; }

        [BsonElement("sponsorId")]
        public string? SponsorId { get; set; }

        [BsonElement("address")]
        public required Address Address { get; set; }

        [BsonElement("platformId")]
        public string? PlatformId { get; set; }

        [BsonElement("profilePictureUrl")]
        public string? ProfilePictureUrl { get; set; }

        [BsonElement("extraBalance")]
        public decimal? ExtraBalance { get; set; }

        [BsonElement("balance")]
        public decimal? Balance { get; set; }
        [BsonElement("blockedBalance")]
        public decimal? BlockedBalance { get; set; }

        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; }

        [BsonElement("cameFrom")]
        public String? CameFrom { get; set; }

        [BsonElement("profession")]
        public String? Profession { get; set; }

        [BsonElement("monthlyIncome")]
        public String? MonthlyIncome { get; set; }

        [BsonElement("withdrawDate")]
        public DateTime? WithdrawDate { get; set; }

        [BsonElement("clientProfit")]
        public double? ClientProfit { get; set; }

        [BsonElement("walletExtract")]
        public WalletExtract WalletExtract { get; set; }

        [BsonElement("password")]
        public string? Password { get; set; }

        [BsonElement("status")]
        public int? Status { get; set; }

        public Client(string id, string name, string email, string phone, string platformID, Address address)
        {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            PlatformId = platformID;
            BlockedBalance = 0;
            Balance = 0;
            ClientProfit = 1.5;
            ExtraBalance = 0;
            WalletExtract = new WalletExtract();
            DateCreated = DateTime.UtcNow;
            Status = 1;
        }

        public void AddToBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser adicionado deve ser positivo.");
            }
            Balance += amount;
        }

        public void AddToExtraBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser adicionado deve ser positivo.");
            }
            ExtraBalance += amount;
        }

        public void AddToBlockedBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser adicionado deve ser positivo.");
            }
            BlockedBalance += amount;
            Console.WriteLine($"Valor de ${amount} adicionado ao Blocked Balance");
        }

        public void WithdrawFromBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser sacado deve ser positivo.");
            }
            if (BlockedBalance > (Balance - amount))
            {
                throw new InvalidOperationException($"Saldo bloqueado, Blocked Balance = {BlockedBalance}, Balance = {Balance}, amount = {amount}");
            }
            if (amount > Balance)
            {
                throw new InvalidOperationException("Saldo insuficiente para realizar o saque.");
            }
            Balance -= amount;
        }

        public void WithdrawFromExtraBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser sacado deve ser positivo.");
            }
            if (amount > ExtraBalance)
            {
                throw new InvalidOperationException("Saldo insuficiente para realizar o saque do Extra.");
            }
            ExtraBalance -= amount;
        }

        public void WithdrawFromBlockedBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser sacado deve ser positivo.");
            }
            if (amount > BlockedBalance)
            {
                throw new InvalidOperationException("Saldo insuficiente para realizar o saque.");
            }
            BlockedBalance -= amount;
        }

    }

    public class WalletExtract
    {
        [BsonElement("purchases")]
        public List<string> Purchases { get; set; }

        [BsonElement("withdrawals")]
        public List<string> Withdrawals { get; set; }

        public WalletExtract()
        {
            Purchases = new List<string>();
            Withdrawals = new List<string>();
        }
    }

}
