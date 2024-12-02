using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DotnetBackend.Models
{
    public class AdminWithdrawal
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? AdminWithdrawalId { get; set; }

        [BsonElement("amountWithdrawn")]
        public required double AmountWithdrawn { get; set; }
        [BsonElement("amountWithdrawnToPay")]
        public required double AmountWithdrawnToPay { get; set; }

        [BsonElement("datacreated")]
        public DateTime? DateCreated { get; set; }

        [BsonElement("status")]
        public int? Status { get; set; } = 1;

        public AdminWithdrawal() { }

        public AdminWithdrawal(double amountWithdrawn)
        {
            AmountWithdrawn = amountWithdrawn;
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateCreated = TimeZoneInfo.ConvertTimeFromUtc(utcNow, brasiliaTimeZone);
        }

        public override string ToString()
        {
            return $"WithdrawalId: {AdminWithdrawalId}, AmountWithdrawn: {AmountWithdrawn}, DateCreated: {DateCreated}, Status: {Status}";
        }
    }
}
