using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models
{
    public class Extract
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? ExtractId { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("clientId")]
        public string? ClientId { get; set; }
        
        [BsonElement("dateCreated")]
        public DateTime? DateCreated { get; set; }

        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; } // Tornado não-nullable

        [BsonElement("status")]
        public int Status { get; set; } // Tornado não-nullable

        // Construtor
        public Extract(string name, decimal totalAmount, string clientId)
        {
            Name = name;
            TotalAmount = totalAmount;
            Status = 1; // Inicializa o status como 1
            ClientId = clientId;
            ExtractId = "E" + Guid.NewGuid().ToString(); // Gera um ID único usando Guid
        }
    }
}
