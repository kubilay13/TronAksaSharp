using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class Receipt
    {
        [JsonPropertyName("net_fee")]
        public long NetFee { get; set; }      // SUN

        [JsonPropertyName("energy_fee")]
        public long EnergyFee { get; set; }   // SUN
    }
}
