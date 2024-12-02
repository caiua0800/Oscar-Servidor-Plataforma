using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models
{
    public class BankAccount
    {
        // Adiciona uma propriedade Id que será usada como o identificador único
        [BsonId]
        public ObjectId Id { get; set; } // O MongoDB usa ObjectId por padrão, que é um tipo do MongoDB

        [BsonElement("accountHolderName")]
        public required string AccountHolderName { get; set; }

        [BsonElement("balance")]
        public decimal? Balance { get; set; }

        // Para a compatibilidade com o construtor padrão do modelo
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