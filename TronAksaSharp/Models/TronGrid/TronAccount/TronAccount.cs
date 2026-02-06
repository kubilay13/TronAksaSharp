using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronAccount
    {
        // Cüzdan adresi (Base58)
        [JsonPropertyName("address")]
        public string Address { get; set; }

        // Cüzdan bakiyesi (SUN)
        // 1 TRX = 1_000_000 SUN
        [JsonPropertyName("balance")]
        public long Balance { get; set; }

        // Cüzdan oluşturulma zamanı (Unix ms)
        [JsonPropertyName("create_time")]
        public long CreateTime { get; set; }

        // Son yapılan işlem zamanı
        [JsonPropertyName("latest_opration_time")]
        public long LatestOperationTime { get; set; }

        // Son resource (bandwidth / energy) harcama zamanı
        [JsonPropertyName("latest_consume_time")]
        public long LatestConsumeTime { get; set; }

        // Son ücretsiz bandwidth kullanım zamanı
        [JsonPropertyName("latest_consume_free_time")]
        public long LatestConsumeFreeTime { get; set; }

        // Son stake / withdraw zamanı
        [JsonPropertyName("latest_withdraw_time")]
        public long LatestWithdrawTime { get; set; }

        // Harcanabilir resource limiti
        [JsonPropertyName("allowance")]
        public long Allowance { get; set; }

        // Dondurulmuş TRX bilgileri (Energy, Bandwidth, Tron Power)
        [JsonPropertyName("frozenV2")]
        public List<TronFrozen> Frozen { get; set; }

        // TRC20 token listesi (TokenAddress -> Amount)
        [JsonPropertyName("trc20")]
        public List<Dictionary<string, string>> Trc20Tokens { get; set; }

        // Energy ve resource bilgileri
        [JsonPropertyName("account_resource")]
        public TronAccountResource AccountResource { get; set; }

        // Owner (sahip) yetkileri
        [JsonPropertyName("owner_permission")]
        public TronPermission OwnerPermission { get; set; }

        // Active (işlem yapma) yetkileri
        [JsonPropertyName("active_permission")]
        public List<TronPermission> ActivePermissions { get; set; }

        // Super Representative oyları
        [JsonPropertyName("votes")]
        public List<TronVote> Votes { get; set; }

        // Bandwidth pencere süresi
        [JsonPropertyName("net_window_size")]
        public int NetWindowSize { get; set; }

        // Bandwidth pencere optimizasyon durumu
        [JsonPropertyName("net_window_optimized")]
        public bool NetWindowOptimized { get; set; }
    }
}
