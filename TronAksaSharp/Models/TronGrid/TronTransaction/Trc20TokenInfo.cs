using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class Trc20TokenInfo
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
