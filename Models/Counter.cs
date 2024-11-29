using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models
{
    public class Counter
    {
        [BsonId]
        public string? Id { get; set; } // Nome do contador, como "purchases"

        [BsonElement("sequenceValue")]
        public int SequenceValue { get; set; } // Valor atual do contador
    }
}
