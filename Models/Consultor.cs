using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DotnetBackend.Models;

public class Consultor
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

    [BsonElement("clientsQtt")]
    public int? ClientsQtt { get; set; }

    [BsonElement("dateCreated")]
    public DateTime DateCreated { get; set; }

    [BsonElement("status")]
    public int? Status { get; set; }

    public Consultor(string id, string name, string email, string phone)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        DateCreated = DateTime.UtcNow;
        Status = 1;
    }
}

