using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;

namespace DotnetBackend.Models
{
    public class Chat
    {

        [BsonId] // Anotação para informar que este é o campo `_id`
        public ObjectId Id { get; set; } // O campo que corresponde ao `_id` no MongoDB

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