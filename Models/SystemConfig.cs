using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;

namespace DotnetBackend.Models;
public class SystemConfig
{
    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("value")]
    public required string Value { get; set; }

    [BsonElement("lastValue")]
    public string? LastValue { get; set; }

    [BsonElement("lastUpdate")]
    public DateTime? LastUpdate { get; set; }

    public SystemConfig(string name, string value, string? lastValue)
    {
        Name = name;
        Value = value;
        LastValue = lastValue;
        LastUpdate = DateTime.UtcNow;
    }

}
