using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronPermission
    {
        // Permission adı (owner / active)
        [JsonPropertyName("permission_name")]
        public string PermissionName { get; set; }

        // Permission tipi
        [JsonPropertyName("type")]
        public string Type { get; set; }

        // İmza eşiği (multi-sig)
        [JsonPropertyName("threshold")]
        public int Threshold { get; set; }

        // Permission ID
        [JsonPropertyName("id")]
        public int Id { get; set; }

        // İzin verilen işlemler (bitmask)
        [JsonPropertyName("operations")]
        public string Operations { get; set; }

        // Yetkili adresler
        [JsonPropertyName("keys")]
        public List<TronPermissionKey> Keys { get; set; }
    }
}
