namespace TronAksaSharp.Models.Domain.TronAccount
{
    public class TronBalanceInfo
    {
        public decimal TrxBalance { get; set; }
        public decimal BandwidthStake { get; set; }
        public decimal EnergyStake { get; set; }
        public decimal BandwidthForTRXStake { get; set; }
        public decimal EnergyForTRXStake { get; set; }
    }
}
