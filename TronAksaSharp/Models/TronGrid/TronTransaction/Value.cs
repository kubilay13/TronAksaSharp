using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class Value
    {
        [JsonPropertyName("owner_address")]
        public string From { get; set; }

        [JsonPropertyName("to_address")]
        public string To { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; } // SUN
    }
}
