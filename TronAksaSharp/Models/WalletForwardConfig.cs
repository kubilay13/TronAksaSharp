using TronAksaSharp.Enums;

namespace TronAksaSharp.Models
{
    public class WalletForwardConfig
    {
        public string WatchAddress { get; set; }
        public string WatchPrivateKey { get; set; }
        public string ForwardAddress { get; set; }
        public TronNetwork Network { get; set; }
        public decimal MinTRXReserve { get; set; } = 1m; // Cüzdanda kalan TRX
        public int CheckIntervalSeconds { get; set; } = 10; // Kaç saniyede bir kontrol
    }
}
