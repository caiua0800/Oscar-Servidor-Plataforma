using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;
public class Message
{
    [BsonElement("dateCreated")]
    public DateTime DateCreated { get; set; }

    [BsonElement("msg")]
    public string Msg { get; set; }

    [BsonElement("clientName")]
    public string ClientName { get; set; }

    [BsonElement("isResponse")]
    public bool? IsResponse { get; set; }

    public Message() { }

    public Message(DateTime dateCreated, string clientName, string msg, bool isResponse)
    {
        DateCreated = dateCreated;
        ClientName = clientName;
        Msg = msg;
        IsResponse = isResponse;
    }
}