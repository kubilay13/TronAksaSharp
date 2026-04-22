using TronAksaSharp.Crypto;
using TronAksaSharp.Endcoding;
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

        // ================= ADRES BYTE UZUNLUĞU SORGULAMA =================
        public static int GetAddressByteLength(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Adres boş olamaz");
            }

            byte[] addrBytes = Base58.Decode(address);
            return addrBytes.Length;
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

        // ================= TRC20 BALANCE =================
        public static async Task<decimal> GetTRC20BalanceAsync(string walletAddress, string contractAddress, int decimals, TronNetwork network)
        {
            return await BalanceService.GetTRC20BalanceAsync(walletAddress, contractAddress, decimals, network);
        }

        // ================= TRX TRANSFER =================
        public static async Task<TransferResult> SendTRXAsync(string fromAddress, string privateKeyHex, string toAddress, decimal amount, TronNetwork network)
        {
            byte[] privateKey = Convert.FromHexString(privateKeyHex);
            long amountInSun = (long)(amount * 1_000_000m);

            var tx = await TronTransferService.CreateTRXTransactionAsync(fromAddress, toAddress, amount, privateKey, network);
            string rawHex = tx.RootElement.GetProperty("raw_data_hex").GetString();
            string signature = TronTransactionSigner.Sign(rawHex, privateKey, fromAddress);

            return await TronTransferService.BroadcastAsync(tx.RootElement, signature, network);
        }

        // ================= TRC20 TRANSFER =================
        public static async Task<TransferResult> SendTRC20Async(string from, string privateKeyHex, string to, string contract, decimal amount, int decimals, TronNetwork network)
        {
            byte[] pk = Convert.FromHexString(privateKeyHex);
            var accountDoc = await TronAccountService.GetAccountAsync(from, network);
            int permId = TronAccountPermissionResolver.ResolvePermissionId(accountDoc, from);

            var txDoc = await TronTransferService.CreateTRC20TransactionAsync(from, to, contract, amount, decimals, permId, network);
            var tx = txDoc.RootElement.GetProperty("transaction");
            string rawHex = tx.GetProperty("raw_data_hex").GetString();
            string signature = TronTransactionSigner.Sign(rawHex, pk, from);

            return await TronTransferService.BroadcastAsync(tx, signature, network);
        }

        // ================= TRONGRID CÜZDAN BİLGİLERİ =================
        public static async Task<TronAccount?> GetTronGridAccountDetailAsync(string address, TronNetwork network, string apiKey = "")
        {
            var service = new TronGridService(apiKey, network);
            return await service.GetAccountAsync(address);
        }

        // ================= TRONGRID FEE PARAMETRESİ =================
        public static async Task<FeeParameters> GetFeeParametersAsync(TronNetwork network, string apiKey = "")
        {
            var service = new TronGridService(apiKey, network);
            return await service.GetTRXFeeParametersAsync();
        }

        // ================= TRONGRID TRX İŞLEM BİLGİLERİ =================
        public static async Task<List<TronTransaction>> GetTronGridTRXTransactionsDetailsAsync(string address, TronNetwork network, int? limit = null, string apiKey = "")
        {
            var service = new TronGridService(apiKey, network);
            return await service.GetTRXTransactiondDetailsAsync(address, limit);
        }

        // ================= TRONGRID TRC20 İŞLEM BİLGİLERİ =================
        public static async Task<List<Trc20Transaction>> GetTronGridTRC20TransactionsDetailsAsync(string address, TronNetwork network, int? limit = null, string apiKey = "")
        {
            var service = new TronGridService(apiKey, network);
            return await service.GetTRC20TransactionDetailsAsync(address, limit);
        }
        // ================= TRON OTOMATİK PARA ÇEKME İŞLEMİ =================

        //public static WalletForwardService CreateForwardService(WalletForwardConfig config)
        //{
        //    return new WalletForwardService(config);
        //}

        public static async Task StartForwardingAsync(string watchAddress, string watchPrivateKey, string forwardAddress, decimal minReserve, decimal maxReserve, int checkIntervalSeconds, TronNetwork network, CancellationToken cancellationToken = default)
        {
            var config = new WalletForwardConfig
            {
                WatchAddress = watchAddress,
                WatchPrivateKey = watchPrivateKey,
                ForwardAddress = forwardAddress,
                Network = network,
                MinTRXReserve = minReserve,
                MaxTRXReserve = maxReserve,
                CheckIntervalSeconds = checkIntervalSeconds
            };

            // ✅ DÜZELTİLDİ: TronClient örneği oluşturup 2 parametre gönderiyoruz
            var tronClient = new TronClient();  // TronClient örneği oluştur
            var service = new WalletForwardService(tronClient, config);  // 2 parametre
            await service.StartAsync(cancellationToken);
        }
    }
}
