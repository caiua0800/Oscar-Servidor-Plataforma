using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DotnetBackend.Models
{
    public class Withdrawal
    {
        internal object purchaseId;

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? WithdrawalId { get; set; }

        [BsonElement("clientId")]
        public required string ClientId { get; set; }

        [BsonElement("amountWithdrawn")]
        public required double AmountWithdrawn { get; set; }

        [BsonElement("amountWithdrawnReceivable")]
        public double? AmountWithdrawnReceivable { get; set; }

        [BsonElement("withdrawnItems")]
        public List<string>? WithdrawnItems { get; set; }

        [BsonElement("datacreated")]
        public DateTime? DateCreated { get; set; }

        [BsonElement("status")]
        public int? Status { get; set; } = 1;

        public Withdrawal() { }

        public Withdrawal(string clientId, double amountWithdrawn)
        {
            ClientId = clientId;
            AmountWithdrawn = amountWithdrawn;
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateCreated = TimeZoneInfo.ConvertTimeFromUtc(utcNow, brasiliaTimeZone);
        }

        public override string ToString()
        {
            return $"WithdrawalId: {WithdrawalId}, ClientId: {ClientId}, AmountWithdrawn: {AmountWithdrawn}, DateCreated: {DateCreated}, Status: {Status}";
        }
    }
}
