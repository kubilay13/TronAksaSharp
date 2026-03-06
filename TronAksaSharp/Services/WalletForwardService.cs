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
            _lastBalance = await BalanceService.GetTRXBalanceAsync(_config.WatchAddress, _config.Network);
            await ForwardIfNeededAsync(_lastBalance);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    decimal currentBalance = await BalanceService.GetTRXBalanceAsync(_config.WatchAddress, _config.Network);

                    if (currentBalance > _lastBalance)
                    {
                        decimal amountToForward = currentBalance - _config.MinTRXReserve;
                        if (amountToForward > 0)
                        {
                            var result = await TronClient.SendTRXAsync(
                                _config.WatchAddress,
                                _config.WatchPrivateKey,
                                _config.ForwardAddress,
                                amountToForward,
                                _config.Network
                            );

                            Console.WriteLine($"Forwarded {amountToForward} TRX. TxID: {result.TxId}");
                        }
                    }

                    _lastBalance = currentBalance;
                    await Task.Delay(5000, cancellationToken); // 5 saniye bekle
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    await Task.Delay(5000, cancellationToken); //hata durumunda 5 saniye bekler
                }
            }
        }

        private async Task ForwardIfNeededAsync(decimal currentBalance)
        {
            decimal amountToForward = currentBalance - _config.MinTRXReserve;

            if (amountToForward > 0)
            {
                var result = await TronClient.SendTRXAsync(
                    _config.WatchAddress,
                    _config.WatchPrivateKey,
                    _config.ForwardAddress,
                    amountToForward,
                    _config.Network
                );

                Console.WriteLine($"Forwarded {amountToForward} TRX. TxID: {result.TxId}");
            }
        }
    }
}
