namespace TronAksaSharp.Models.Domain.TronAccount
{
    public class TronBalanceInfo
    {
        public decimal TrxBalance { get; set; }           
        public decimal BandwidthForTRXStake { get; set; } 
        public decimal EnergyForTRXStake { get; set; }    
        public long BandwidthStake { get; set; }          
        public long EnergyStake { get; set; }
    }
}
