using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models;

public class Indication
{

    [BsonId]

    public string? Id { get; set; }

    [BsonElement("clientId")]
    public string ClientId { get; set; }

    [BsonElement("clientRef")]
    public string? ClientRef { get; set; }

    [BsonElement("purchaseValue")]
    public double? PurchaseValue { get; set; }

    [BsonElement("value")]
    public double Value { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("dateCreated")]
    public DateTime? DateCreated { get; set; }

    public Indication(string clientId, string description, double value)
    {
        ClientId = clientId;
        Description = description;
        Value = value;
    }
}