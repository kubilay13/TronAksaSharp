using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronFrozen
    { 
        // Dondurulan miktar (SUN)
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        // Freeze tipi: ENERGY / BANDWIDTH / TRON_POWER
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
