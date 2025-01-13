using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models
{
    public class PasswordResetToken
    {
        [BsonId]
        public ObjectId Id { get; set; } 

        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}