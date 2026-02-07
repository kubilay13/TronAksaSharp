using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class RawData
    {
        [JsonPropertyName("contract")]
        public List<Contract> Contract { get; set; }
    }
}
