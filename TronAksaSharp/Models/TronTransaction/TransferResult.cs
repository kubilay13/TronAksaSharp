namespace TronAksaSharp.Models.TronTransaction
{
    public class TransferResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? TxId { get; set; }
    }
}
