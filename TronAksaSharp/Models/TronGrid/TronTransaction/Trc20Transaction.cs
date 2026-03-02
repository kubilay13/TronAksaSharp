using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class Trc20Transaction
    {
        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("token_info")]
        public Trc20TokenInfo TokenInfo { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonIgnore]
        public decimal Amount => TokenInfo != null && decimal.TryParse(Value, out var raw)
            ? raw / (decimal)Math.Pow(10, TokenInfo.Decimals)
            : 0;

        [JsonIgnore]
        public string Status { get; set; }

        [JsonIgnore]
        public decimal Fee { get; set; }

        [JsonIgnore]
        public DateTime Timestamp { get; set; }
    }
} 


