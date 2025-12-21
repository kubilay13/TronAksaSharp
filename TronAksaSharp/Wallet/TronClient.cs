using TronAksaSharp.Enums;
using TronAksaSharp.TronCrypto;
using TronAksaSharp.Wallet.Services;

namespace TronAksaSharp.Wallet
{
    public class TronClient
    {
        public static async Task<bool> SendTRXAsync(
            string fromAddress,
            string privateKeyHex,
            string toAddress,
            decimal amount,
            TronNetwork network)
        {
            byte[] privateKey = Convert.FromHexString(privateKeyHex);

            var tx = await TronTransferService.CreateTRXTransactionAsync(
                fromAddress,
                toAddress,
                amount,
                network
            );

            string rawHex = tx.RootElement
                .GetProperty("raw_data_hex")
                .GetString();

            string signature = TronTransactionSigner.Sign(rawHex, privateKey);

            return await TronTransferService.BroadcastAsync(
                tx,
                signature,
                network
            );
        }
    }
}
