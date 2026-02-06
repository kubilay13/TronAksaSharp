using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronAccountResource
    {
        // Energy pencere optimizasyon durumu
        [JsonPropertyName("energy_window_optimized")]
        public bool EnergyWindowOptimized { get; set; }

        // Son energy harcama zamanı
        [JsonPropertyName("latest_consume_time_for_energy")]
        public long LatestConsumeTimeForEnergy { get; set; }

        // Energy pencere süresi (ms)
        [JsonPropertyName("energy_window_size")]
        public long EnergyWindowSize { get; set; }
    }
}
