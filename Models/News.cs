

using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models;

public class News
{
    [BsonId]
    public string? Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }
    [BsonElement("text")]
    public string Text { get; set; }
    [BsonElement("urlMidea")]
    public string UrlMidea { get; set; }
    [BsonElement("urlDestin")]
    public string UrlDestin { get; set; }
    [BsonElement("dateCreated")]
    public DateTime? DateCreated { get; set; }
}