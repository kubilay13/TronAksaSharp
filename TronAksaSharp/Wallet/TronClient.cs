using TronAksaSharp.Enums;
using TronAksaSharp.Models;
using TronAksaSharp.Services;
using TronAksaSharp.TronCrypto;

namespace TronAksaSharp.Wallet
{
    public static class TronClient
    {
        // ================= TRON WALLET =================
        public static Models.Wallet CreateTronWallet()
        {
            var privateKey = KeyGenerator.GeneratePrivateKey(); // Yeni bir özel anahtar oluşturur
            var publicKey = KeyGenerator.PrivateKeyToPublicKey(privateKey); // Özel anahtardan genel anahtar türetir
            var address = AddressGenerator.PublicKeyToAddress(publicKey); // Genel anahtardan Tron adresi oluşturur

            return new Models.Wallet
            {
                PrivateKey = privateKey,
                PublicKey = publicKey,
                Address = address
            };
        }
        // ================= BALANCE-STAKE =================
        public static async Task<TronBalanceInfo> GetBalancesAsync(string address, TronNetwork network)
        {
            return new TronBalanceInfo
            {
                TrxBalance = await BalanceService.GetTRXBalanceAsync(address, network),
                BandwidthStake = await BalanceService.GetBandwidthStakeAsync(address, network),
                EnergyStake = await BalanceService.GetEnergyStakeAsync(address, network)
            };
        }

        // ================= TRC20 STAKE =================
        public static async Task<decimal> GetTRC20BalanceAsync(
            string walletAddress,
            string contractAddress,
            int decimals,
            TronNetwork network)
        {
            return await BalanceService.GetTRC20BalanceAsync(
                walletAddress,
                contractAddress,
                decimals,
                network
            );
        }
        // ================= TRX =================
        public static async Task<TransferResult> SendTRXAsync(string from, string privateKeyHex, string to, decimal amount, TronNetwork network)
        {
            byte[] pk = Convert.FromHexString(privateKeyHex);

            var txDoc = await TronTransferService.CreateTRXTransactionAsync(from, to, amount, network);

            string rawHex = txDoc.RootElement.GetProperty("raw_data_hex").GetString();

            string signature = TronTransactionSigner.Sign(rawHex, pk);

            return await TronTransferService.BroadcastAsync(txDoc.RootElement, signature, network);
        }

        // ================= TRC20 =================
        public static async Task<TransferResult> SendTRC20Async(string from, string privateKeyHex, string to, string contract, decimal amount, int decimals, TronNetwork network)
        {
            byte[] pk = Convert.FromHexString(privateKeyHex);

            var txDoc = await TronTransferService.CreateTRC20TransactionAsync(
                from, to, contract, amount, decimals, network);

            var tx = txDoc.RootElement.GetProperty("transaction");

            string rawHex = tx.GetProperty("raw_data_hex").GetString();
            string signature = TronTransactionSigner.Sign(rawHex, pk);

            return await TronTransferService.BroadcastAsync(tx, signature, network);
        }
    }
}
