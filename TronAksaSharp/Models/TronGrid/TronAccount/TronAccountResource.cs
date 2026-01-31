namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronAccountResource
    {
        // Energy penceresi optimize mi
        public bool EnergyWindowOptimized { get; set; }

        // Son energy harcama zamanı
        public long LatestConsumeTimeForEnergy { get; set; }

        // Energy pencere süresi
        public long EnergyWindowSize { get; set; }
    }

}
