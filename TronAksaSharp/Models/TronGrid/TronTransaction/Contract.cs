using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class Contract
    {
        [JsonPropertyName("parameter")]
        public Parameter Parameter { get; set; }
    }
}
