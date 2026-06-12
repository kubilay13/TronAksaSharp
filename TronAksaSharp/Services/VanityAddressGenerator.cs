using System.Diagnostics;
using TronAksaSharp.TronCrypto;

namespace TronAksaSharp.Services
{
    public static class VanityAddressGenerator
    {
        public static async Task<TronAksaSharp.Models.Domain.TronAccount.Wallet?> GenerateVanityAddressAsync(
            string containsText,
            int? maxAttempts = null,
            int progressInterval = 10000)
        {
            // Küçük harfleri büyük yap (Tron adresleri büyük harf + rakam)
            var searchText = containsText.ToUpperInvariant();

            // İstatistikler
            var stopwatch = Stopwatch.StartNew();
            long attemptCount = 0;
            var lastUpdate = DateTime.Now;

            return await Task.Run(() =>
            {
                while (true)
                {
                    attemptCount++;

                    // Max deneme kontrolü
                    if (maxAttempts.HasValue && attemptCount > maxAttempts.Value)
                    {
                        return null;
                    }

                    // Rastgele özel anahtar üret
                    var privateKey = KeyGenerator.GeneratePrivateKey();
                    var publicKey = KeyGenerator.PrivateKeyToPublicKey(privateKey);
                    var address = AddressGenerator.PublicKeyToAddress(publicKey);

                    // Adres içinde aranan kelime var mı?
                    if (address.Contains(searchText))
                    {
                        stopwatch.Stop();
                        return new TronAksaSharp.Models.Domain.TronAccount.Wallet
                        {
                            PrivateKey = privateKey,
                            PublicKey = publicKey,
                            Address = address
                        };
                    }

                    // İlerleme göster
                    if (attemptCount % progressInterval == 0)
                    {
                        var elapsed = stopwatch.Elapsed;
                        var speed = attemptCount / elapsed.TotalSeconds;
                        var elapsedFormatted = FormatTime(elapsed);

                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Deneme: {attemptCount:N0} | Hız: {speed:N0} adet/s | Süre: {elapsedFormatted}");
                        Console.WriteLine($"          Son adres: {address}");
                    }
                }
            });
        }

        private static string FormatTime(TimeSpan time)
        {
            if (time.TotalDays >= 1)
                return $"{time.Days}g {time.Hours}s {time.Minutes}d";
            if (time.TotalHours >= 1)
                return $"{time.Hours}s {time.Minutes}d {time.Seconds}sn";
            if (time.TotalMinutes >= 1)
                return $"{time.Minutes}d {time.Seconds}sn";
            return $"{time.Seconds}sn";
        }
    }
}

