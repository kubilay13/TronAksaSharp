using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class TronTransaction
    {

        [JsonPropertyName("txID")]
        public string TxId { get; set; }

        [JsonPropertyName("block_timestamp")]
        public long BlockTimestamp { get; set; }

        [JsonPropertyName("ret")]
        public List<TronTransactionResult> Result { get; set; }
        // SUCCESS / FAILED

        [JsonPropertyName("raw_data")]
        public RawData RawData { get; set; }
        // from / to / amount burada

        [JsonPropertyName("receipt")]
        public Receipt Receipt { get; set; }
        // fee burada
        [JsonIgnore]
        public decimal FeeTRX => ((Receipt?.NetFee ?? 0) + (Receipt?.EnergyFee ?? 0)) / 1_000_000m;
    }
}
