using System.Text.Json;
using TronAksaSharp.Models.TronTransaction;

namespace TronAksaSharp.Abstractions
{
    public interface ITronTransferService
    {
        /// <summary>TRX işlemi oluşturur</summary>
        Task<JsonDocument> CreateTRXTransactionAsync(string fromAddress, string toAddress, decimal amountTrx, byte[] privateKey);

        /// <summary>TRC-20 işlemi oluşturur</summary>
        Task<TransferResult> BroadcastAsync(JsonElement tx, string signatureHex);

        /// <summary>İşlemi broadcast eder (blok zincirine gönderir)</summary>
        Task<JsonDocument> CreateTRC20TransactionAsync(string fromAddress, string toAddress, string contractAddress, decimal amount, int decimals, int permissionId);
    }
}
