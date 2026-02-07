using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class TronTransactionWrapper
    {
        [JsonPropertyName("data")]
        public List<TronTransaction> Data { get; set; }
    }
}
