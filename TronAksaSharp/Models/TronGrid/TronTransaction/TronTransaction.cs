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
    }
}
