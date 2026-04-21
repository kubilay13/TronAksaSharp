using TronAksaSharp.Models;
using TronAksaSharp.Wallet;

namespace TronAksaSharp.Services
{
    public class WalletForwardService
    {
        private readonly TronClient _tronClient;
        private readonly WalletForwardConfig _config;
        private decimal _lastBalance;

        public WalletForwardService(TronClient tronClient, WalletForwardConfig config)
        {
            _tronClient = tronClient;
            _config = config;
            _lastBalance = 0;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Başlangıç bakiyesini al
            _lastBalance = await BalanceService.GetTRXBalanceAsync(_config.WatchAddress, _config.Network);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Başlangıç bakiyesi: {_lastBalance} TRX");

            // İlk kontrol (zaten para varsa onu da gönder)
            await ForwardIfNeededAsync(_lastBalance);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Güncel bakiyeyi al
                    decimal currentBalance = await BalanceService.GetTRXBalanceAsync(_config.WatchAddress, _config.Network);

                    // Yeni para geldiyse
                    if (currentBalance > _lastBalance)
                    {
                        decimal gelenMiktar = currentBalance - _lastBalance;
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 💰 {gelenMiktar} TRX geldi!");

                        // Gönderilecek miktar = tüm bakiye - reserve
                        decimal amountToForward = currentBalance - _config.MinTRXReserve;
                        decimal maxAmountToForward = _config.MaxTRXReserve;
                        if(maxAmountToForward > 0 || maxAmountToForward!=null)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📤 {maxAmountToForward} TRX gönderiliyor...");
                            var result = await TronClient.SendTRXAsync(
                                _config.WatchAddress,
                                _config.WatchPrivateKey,
                                _config.ForwardAddress,
                                maxAmountToForward,
                                _config.Network
                            );

                            if (result.Success)
                            {
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Gönderildi! TxID: {result.TxId}");
                            }
                            else
                            {
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Hata: {result.Error}");
                            }
                        }

                        if (amountToForward > 0)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📤 {amountToForward} TRX gönderiliyor...");

                            var result = await TronClient.SendTRXAsync(
                                _config.WatchAddress,
                                _config.WatchPrivateKey,
                                _config.ForwardAddress,
                                amountToForward,
                                _config.Network
                            );

                            if (result.Success)
                            {
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Gönderildi! TxID: {result.TxId}");
                            }
                            else
                            {
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Hata: {result.Error}");
                            }
                        }
                    }

                    // Son bakiyeyi güncelle
                    _lastBalance = currentBalance;

                    // Config'deki süre kadar bekle
                    await Task.Delay(_config.CheckIntervalSeconds * 1000, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Hata: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }
        private async Task ForwardIfNeededAsync(decimal currentBalance)
        {
            decimal amountToForward = currentBalance - _config.MinTRXReserve;
            decimal maxAmountToForward = _config.MaxTRXReserve;

            if (maxAmountToForward > 0 || maxAmountToForward != null)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📤 {maxAmountToForward} TRX gönderiliyor...");
                var result = await TronClient.SendTRXAsync(
                    _config.WatchAddress,
                    _config.WatchPrivateKey,
                    _config.ForwardAddress,
                    maxAmountToForward,
                    _config.Network
                );

                if (result.Success)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Gönderildi! TxID: {result.TxId}");
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Hata: {result.Error}");
                }
            }

            if (amountToForward > 0)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📤 İlk transfer: {amountToForward} TRX");

                var result = await TronClient.SendTRXAsync(
                    _config.WatchAddress,
                    _config.WatchPrivateKey,
                    _config.ForwardAddress,-
                    amountToForward,
                    _config.Network
                );

                if (result.Success)
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ İlk transfer tamam! TxID: {result.TxId}");
                else
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ İlk transfer hatası: {result.Error}");
            }
        }
    }
}
