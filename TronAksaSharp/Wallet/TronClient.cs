using TronAksaSharp.Crypto;
using TronAksaSharp.Enums;
using TronAksaSharp.Models.Domain.TronAccount;
using TronAksaSharp.Models.TronGrid.TronAccount;
using TronAksaSharp.Models.TronGrid.TronTransaction;
using TronAksaSharp.Models.TronTransaction;
using TronAksaSharp.Services;
using TronAksaSharp.TronCrypto;

namespace TronAksaSharp.Wallet
{
    public class TronClient
    {
        private readonly TronGridService _tronGridService;
        public TronClient(string apiKey, TronNetwork tronNetwork)
        {
            _tronGridService = new TronGridService(apiKey, tronNetwork);
        }

        // ================= TRON WALLET =================
        public static Models.Domain.TronAccount.Wallet CreateTronWallet()
        {
            var privateKey = KeyGenerator.GeneratePrivateKey(); // Yeni bir özel anahtar oluşturur
            var publicKey = KeyGenerator.PrivateKeyToPublicKey(privateKey); // Özel anahtardan genel anahtar türetir
            var address = AddressGenerator.PublicKeyToAddress(publicKey); // Genel anahtardan Tron adresi oluşturur

            return new Models.Domain.TronAccount.Wallet
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
        public static async Task<decimal> GetTRC20BalanceAsync(string walletAddress, string contractAddress, int decimals, TronNetwork network)
        {
            return await BalanceService.GetTRC20BalanceAsync(walletAddress, contractAddress, decimals, network);
        }
        // ================= TRX =================
        public static async Task<TransferResult> SendTRXAsync(string fromAddress, string privateKeyHex, string toAddress, decimal amount, TronNetwork network)
        {
            byte[] privateKey = Convert.FromHexString(privateKeyHex);

            // 1️⃣ TX oluştur (permission içeride çözülüyor)
            var tx = await TronTransferService.CreateTRXTransactionAsync(fromAddress, toAddress, amount, privateKey, network);

            string rawHex = tx.RootElement.GetProperty("raw_data_hex").GetString();

            string signature = TronTransactionSigner.Sign(rawHex, privateKey, fromAddress);

            return await TronTransferService.BroadcastAsync(tx.RootElement, signature, network);
        }

        // ================= TRC20 =================
        public static async Task<TransferResult> SendTRC20Async(string from, string privateKeyHex, string to, string contract, decimal amount, int decimals, TronNetwork network)
        {
            byte[] pk = Convert.FromHexString(privateKeyHex);

            var accountDoc = await TronAccountService.GetAccountAsync(from, network);

            int permId = TronAccountPermissionResolver.ResolvePermissionId(accountDoc, from);

            Console.WriteLine($"FROM    = {from}");
            Console.WriteLine($"SIGNER  = {from}");
            Console.WriteLine($"PERM_ID = {permId}");

            var txDoc = await TronTransferService.CreateTRC20TransactionAsync(from, to, contract, amount, decimals, permId, network);

            var tx = txDoc.RootElement.GetProperty("transaction");

            string rawHex = tx.GetProperty("raw_data_hex").GetString();

            string signature = TronTransactionSigner.Sign(rawHex, pk, from);

            return await TronTransferService.BroadcastAsync(tx, signature, network);
        }

        // ================= TRONGRİD CÜZDAN BİLGİLERİ =================
        public async Task<TronAccount?> GetTronGridAccountDetailAsync(string address)
        {
            return await _tronGridService.GetAccountAsync(address);
        }

        // ================= TRONGRİD İŞLEM BİLGİLERİ =================
        public Task<List<TronTransaction>> GetTronGridTransactionDetailsAsync(string address, int? limit = null)
        {
            return _tronGridService.GetTransactiondDetailsAsync(address, limit);
        }
    }
}
