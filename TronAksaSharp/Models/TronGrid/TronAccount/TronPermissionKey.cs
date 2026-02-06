using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronPermissionKey
    {
        // Yetkili adres
        [JsonPropertyName("address")]
        public string Address { get; set; }

        // İmza ağırlığı (multi-sig)
        [JsonPropertyName("weight")]
        public int Weight { get; set; }
    }
}
