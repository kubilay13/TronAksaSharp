namespace TronAksaSharp.Models.TronTransaction
{
    public class TransactionInfo
    {
        public string TxId { get; set; }
        public long BlockNumber { get; set; }
        public long Timestamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string Asset { get; set; }
        public long Fee { get; set; }
        public long EnergyUsed { get; set; }
        public long NetFee { get; set; }
        public string Result { get; set; }
    }
}
