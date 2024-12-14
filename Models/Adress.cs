using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DotnetBackend.Models;
namespace DotnetBackend.Models;


public class Address
{
    [BsonElement("street")]
    public string? Street { get; set; }
    [BsonElement("number")]
    public string? Number { get; set; }
    [BsonElement("zipcode")]
    public string? Zipcode { get; set; }

    [BsonElement("neighborhood")]
    public string? Neighborhood { get; set; }
    [BsonElement("city")]
    public string? City { get; set; }
    [BsonElement("state")]
    public string? State { get; set; }
    [BsonElement("country")]
    public string? Country { get; set; }

    public Address(string street, string number, string zipcode, string city, string state)
    {
        Street = street; // Inicializa a rua
        Number = number; // Inicializa o número
        Zipcode = zipcode; // Inicializa o CEP
        City = city; // Inicializa a cidade
        State = state; // Inicializa o estado
    }
}
