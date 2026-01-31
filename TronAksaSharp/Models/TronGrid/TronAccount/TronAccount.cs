using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronAccount
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("balance")]
        public long Balance { get; set; } // SUN

        [JsonPropertyName("create_time")]
        public long CreateTime { get; set; }

        [JsonPropertyName("latest_opration_time")]
        public long LatestOperationTime { get; set; }

        [JsonPropertyName("latest_consume_time")]
        public long LatestConsumeTime { get; set; }

        [JsonPropertyName("latest_consume_free_time")]
        public long LatestConsumeFreeTime { get; set; }

        [JsonPropertyName("latest_withdraw_time")]
        public long LatestWithdrawTime { get; set; }

        [JsonPropertyName("allowance")]
        public long Allowance { get; set; }

        [JsonPropertyName("frozenV2")]
        public List<TronFrozen> Frozen { get; set; }

        [JsonPropertyName("trc20")]
        public List<Dictionary<string, string>> Trc20Tokens { get; set; }

        [JsonPropertyName("account_resource")]
        public TronAccountResource AccountResource { get; set; }

        [JsonPropertyName("owner_permission")]
        public TronPermission OwnerPermission { get; set; }

        [JsonPropertyName("active_permission")]
        public List<TronPermission> ActivePermissions { get; set; }

        [JsonPropertyName("votes")]
        public List<TronVote> Votes { get; set; }

        [JsonPropertyName("net_window_size")]
        public int NetWindowSize { get; set; }

        [JsonPropertyName("net_window_optimized")]
        public bool NetWindowOptimized { get; set; }
    }
}
