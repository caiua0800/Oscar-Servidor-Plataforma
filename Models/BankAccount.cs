using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models
{
    public class BankAccount
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("accountHolderName")]
        public required string AccountHolderName { get; set; }

        [BsonElement("balance")]
        public decimal? Balance { get; set; }

        public BankAccount() { }

        public BankAccount(string accountHolderName, decimal? balance)
        {
            AccountHolderName = accountHolderName;
            Balance = balance;
        }

        public void AddToBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser adicionado deve ser positivo.");
            }
            Balance += amount;
        }

        public void WithdrawFromBalance(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("O valor a ser sacado deve ser positivo.");
            }
            if (amount > Balance)
            {
                throw new InvalidOperationException("Saldo insuficiente para realizar o saque.");
            }
            Balance -= amount;
        }
    }
}