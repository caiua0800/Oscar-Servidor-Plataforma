namespace DotnetBackend.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Venda
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    [BsonElement("productName")]
    public required string ProductName { get; set; }

    [BsonElement("quantity")]
    public required double Quantity { get; set; }

    [BsonElement("unityPrice")]
    public required double UnityPrice { get; set; }

    [BsonElement("totalAmount")]
    public double TotalAmount { get; set; }

    [BsonElement("fee")]
    public required double Fee { get; set; }

    [BsonElement("fromWho")]
    public required string FromWho { get; set; }

    [BsonElement("toWho")]
    public string? ToWho { get; set; } // Should not be required

    [BsonElement("status")]
    public int Status { get; set; } = 1; // Assign a default value

    // Construtor padrão
    public Venda() {}

    public Venda(string productName, double quantity, double unityPrice, double fee, string fromWho)
    {
        ProductName = productName;
        this.Quantity = quantity;
        this.UnityPrice = unityPrice;
        this.TotalAmount = (quantity * unityPrice) - ((quantity * unityPrice) * fee);
        this.Fee = fee;
        this.FromWho = fromWho;
    }
}


public class Oferta
{
    public string? Id { get; set; }
    public required string? ClientId { get; set; }
    public required string? ToWho { get; set; }
    public required string? ClientName { get; set; }
    public required double? Price { get; set; }
    public int? Status { get; set; }


    public Oferta(string toWho, string clientId, string clientName, double price)
    {
        ToWho = toWho;
        ClientId = clientId;
        ClientName = clientName;
        Price = price;
        Status = 1;
    }
}