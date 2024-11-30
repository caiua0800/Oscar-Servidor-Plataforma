using System.Text.Json.Serialization;

namespace DotnetBackend.Models
{
    public class PixResponse
    {
        [JsonPropertyName("id")]
        public object Id { get; set; } = string.Empty; // Alterado para objeto

        [JsonPropertyName("point_of_interaction")]
        public PointOfInteraction PointOfInteraction { get; set; } = new PointOfInteraction();
    }

    public class PointOfInteraction
    {
        [JsonPropertyName("transaction_data")]
        public TransactionData TransactionData { get; set; } = new TransactionData();
    }

    public class TransactionData
    {
        [JsonPropertyName("ticket_url")]
        public string TicketUrl { get; set; } = string.Empty;

        [JsonPropertyName("qr_code")]
        public string QrCode { get; set; } = string.Empty;

        [JsonPropertyName("qr_code_base64")]
        public string QrCodeBase64 { get; set; } = string.Empty;
    }
}