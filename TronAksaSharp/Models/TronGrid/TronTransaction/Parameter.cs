using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class Parameter
    {
        [JsonPropertyName("value")]
        public Value Value { get; set; }
    }
}
