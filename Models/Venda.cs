namespace DotnetBackend.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Venda
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    [BsonElement("sellerId")]
    public required string SellerId { get; set; }

    [BsonElement("buyerId")]
    public string? BuyerId { get; set; }

    [BsonElement("totalPrice")]
    public decimal TotalPrice { get; set; }

    [BsonElement("contractPrice")]
    public decimal ContractPrice { get; set; }

    [BsonElement("quantity")]
    public int? Quantity { get; set; }

    [BsonElement("dateCreated")]
    public DateTime? DateCreated { get; set; }

    [BsonElement("unityPrice")]
    public decimal UnityPrice { get; set; }

    [BsonElement("fee")]
    public double? Fee { get; set; }

    [BsonElement("status")]
    public int Status { get; set; } = 1;

    [BsonElement("purchaseId")]
    public string PurchaseId { get; set; }

    [BsonElement("endContractDate")]
    public DateTime? EndContractDate { get; set; }

    [BsonElement("finalIncome")]
    public decimal? FinalIncome { get; set; }

    [BsonElement("buySolicitations")]
    public List<BuySolicitation>? BuySolicitations { get; set; }

    public Venda() { }

    public Venda(string sellerId, decimal totalPrice, decimal unityPrice, string purchaseId, decimal contractPrice)
    {
        SellerId = sellerId;
        TotalPrice = totalPrice;
        UnityPrice = unityPrice;
        PurchaseId = purchaseId;
        ContractPrice = contractPrice;
    }
}
