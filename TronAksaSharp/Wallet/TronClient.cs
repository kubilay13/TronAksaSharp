using TronAksaSharp.Abstractions;
using TronAksaSharp.Crypto;
using TronAksaSharp.Enums;
using TronAksaSharp.Models;
using TronAksaSharp.Models.Domain.TronAccount;
using TronAksaSharp.Models.TronGrid;
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
        private readonly ITronAccountService _tronAccountService;
        private readonly ITronTransferService _tronTransferService;
        private readonly TronNetwork _network;
        public TronClient(ITronAccountService tronAccountService, ITronTransferService tronTransferService, TronNetwork tronNetwork, string apiKey = "")
        {
            _tronAccountService = tronAccountService;
            _tronTransferService = tronTransferService;
            _network = tronNetwork;
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
        public async Task<TransferResult> SendTRXAsync(string fromAddress, string privateKeyHex, string toAddress, decimal amount)
        {
            byte[] privateKey = Convert.FromHexString(privateKeyHex);

            long amountInSun = (long)(amount * 1_000_000m);

            // TX oluştur (permission içeride çözülüyor)
            var tx = await _tronTransferService.CreateTRXTransactionAsync(fromAddress, toAddress, amount, privateKey);

            string rawHex = tx.RootElement.GetProperty("raw_data_hex").GetString();

            string signature = TronTransactionSigner.Sign(rawHex, privateKey, fromAddress);

            return await _tronTransferService.BroadcastAsync(tx.RootElement, signature);
        }

        // ================= TRC20 =================
        public async Task<TransferResult> SendTRC20Async(string from, string privateKeyHex, string to, string contract, decimal amount, int decimals)
        {
            byte[] pk = Convert.FromHexString(privateKeyHex);

            var accountDoc = await _tronAccountService.GetAccountAsync(from, _network);

            int permId = TronAccountPermissionResolver.ResolvePermissionId(accountDoc, from);

            Console.WriteLine($"FROM    = {from}");
            Console.WriteLine($"SIGNER  = {from}");
            Console.WriteLine($"PERM_ID = {permId}");

            var txDoc = await _tronTransferService.CreateTRC20TransactionAsync(from, to, contract, amount, decimals, permId);

            var tx = txDoc.RootElement.GetProperty("transaction");

            string rawHex = tx.GetProperty("raw_data_hex").GetString();

            string signature = TronTransactionSigner.Sign(rawHex, pk, from);

            return await _tronTransferService.BroadcastAsync(tx, signature);
        }

        // ================= TRONGRİD CÜZDAN BİLGİLERİ =================
        public async Task<TronAccount?> GetTronGridAccountDetailAsync(string address)
        {
            return await _tronGridService.GetAccountAsync(address);
        }
        // ================= TRONGRİD ANLIK FEE PARAMETRESİ =================
        public async Task<FeeParameters> GetFeeParametersAsync()
        {
            return await _tronGridService.GetTRXFeeParametersAsync();
        }
        // ================= TRONGRİD TRX İŞLEM BİLGİLERİ =================
        public Task<List<TronTransaction>> GetTronGridTRXTransactionsDetailsAsync(string address, int? limit = null)
        {
            return _tronGridService.GetTRXTransactiondDetailsAsync(address, limit);
        }

        // ================= TRONGRİD TRC20 İŞLEM BİLGİLERİ =================
        public Task<List<Trc20Transaction>> GetTronGridTRC20TransactionsDetailsAsync(string address, int? limit = null)
        {
            return _tronGridService.GetTRC20TransactionDetailsAsync(address, limit);
        }

        // ================= TRON OTOMATİK PARA ÇEKME İŞLEMİ =================
        public WalletForwardService CreateForwardService(WalletForwardConfig config)
        {
            return new WalletForwardService(this, config);
        }
        public async Task StartForwardingAsync(string watchAddress, string watchPrivateKey, string forwardAddress, decimal minReserve,decimal maxReserve , int checkIntervalSeconds, CancellationToken cancellationToken = default)
        {
            var config = new WalletForwardConfig
            {
                WatchAddress = watchAddress,
                WatchPrivateKey = watchPrivateKey,
                ForwardAddress = forwardAddress,
                Network = _network,
                MinTRXReserve = minReserve,
                MaxTRXReserve = maxReserve,
                CheckIntervalSeconds = checkIntervalSeconds
            };

            var service = new WalletForwardService(this, config);
            await service.StartAsync(cancellationToken);
        }
    }
}
