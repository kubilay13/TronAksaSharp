using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class TronTransactionResult
    {
        [JsonPropertyName("contractRet")]
        public string Status { get; set; } // SUCCESS / FAILED
    }
}
