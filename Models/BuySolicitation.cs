using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class BuySolicitation
{

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    [BsonElement("clientId")]
    public string ClientId { get; set; }

    [BsonElement("vendaId")]
    public string VendaId { get; set; }

    [BsonElement("purchaseId")]
    public string? PurchaseId { get; set; }

    [BsonElement("contractPrice")]
    public decimal? ContractPrice { get; set; }

    [BsonElement("totalPrice")]
    public decimal? TotalPrice { get; set; }

    [BsonElement("finalIncome")]
    public decimal? FinalIncome { get; set; }

    [BsonElement("status")]
    public int? Status { get; set; } = 1;

    [BsonElement("dateCreated")]
    public DateTime? DateCreated { get; set; }

    [BsonElement("ticketPayment")]
    public string? TicketPayment { get; set; }

    [BsonElement("ticketId")]
    public string? TicketId { get; set; }

    [BsonElement("qrCode")]
    public string? QrCode { get; set; }

    [BsonElement("expirationDate")]
    public string? ExpirationDate { get; set; }

    public BuySolicitation(string clientId, string vendaId)
    {
        ClientId = clientId;
        VendaId = vendaId;
        Status = 1;
    }
}