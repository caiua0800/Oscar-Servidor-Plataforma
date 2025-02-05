using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DotnetBackend.Models;
using MongoDB.Driver;
namespace DotnetBackend.Models;

public class Admin
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; set; }

    [BsonElement("adminName")]
    public required string Name { get; set; }

    [BsonElement("password")]
    public required string Password { get; set; }

    [BsonElement("platformId")]
    public required string PlatformId { get; set; }

    [BsonElement("permissionLevel")]
    public string? PermissionLevel { get; set; }

    [BsonElement("permission")]
    public string? Permission { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("phone")]
    public string? Phone { get; set; }

    [BsonElement("address")]
    public Address? Address { get; set; }


    public Admin(string id, string name, string email, string phone, string platformID, string password, Address address)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        this.PlatformId = platformID;
        Password = password;
    }
}
