using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;

namespace DotnetBackend.Models
{
    public class Chat
    {

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("clientId")]
        public required string ClientId { get; set; }

        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();

        public Chat() { }

        public Chat(string clientId, DateTime dateCreated)
        {
            ClientId = clientId;
            DateCreated = dateCreated;
        }
    }
}