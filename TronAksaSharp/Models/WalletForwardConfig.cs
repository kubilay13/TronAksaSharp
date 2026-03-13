using TronAksaSharp.Enums;

namespace TronAksaSharp.Models
{
    public class WalletForwardConfig
    {
        // Temel ayarlar
        public string WatchAddress { get; set; }        // İzlenecek cüzdan
        public string WatchPrivateKey { get; set; }     // Private key'i
        public string ForwardAddress { get; set; }       // Hedef cüzdan
        public TronNetwork Network { get; set; }         // Hangi ağ

        // Bakiye yönetimi
        public decimal MinTRXReserve { get; set; }     // Cüzdanda KALMASI GEREKEN minimum TRX
        public decimal MaxTRXReserve { get; set; }   // Cüzdanda KALABİLECEK maksimum TRX

        // Zamanlama
        public int CheckIntervalSeconds { get; set; }  // Kaç saniyede bir kontrol




        // ---------------------------Yakında Gelecek Olanlar----------------------------
        public AutoTransferTiming TransferTiming { get; set; } = AutoTransferTiming.Instant;  // Ne zaman transfer etsin
        public int? TransferDelayMinutes { get; set; }      // Kaç dakika sonra transfer et (Delayed seçilirse)
        public DateTime? ScheduledTransferTime { get; set; } // Belirli bir zamanda transfer et
        // Fee yönetimi
        public bool AutoCalculateFee { get; set; } // Otomatik fee hesaplama
        public decimal FixedFee { get; set; }      // Sabit fee (auto false ise)
        public decimal? MaxFee { get; set; }                 // Maksimum fee limiti
    }
}

