using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;

namespace DotnetBackend.Models;
public class SystemConfig
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("value")]
    public string? Value { get; set; }

    [BsonElement("lastValue")]
    public string? LastValue { get; set; }

    [BsonElement("lastUpdate")]
    public DateTime? LastUpdate { get; set; }

    public SystemConfig(string name, string value)
    {
        Name = name;
        Value = value;
        LastUpdate = DateTime.UtcNow;
    }

}
