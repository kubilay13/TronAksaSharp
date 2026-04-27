using System.Text.Json;

namespace TronAksaSharp.Services
{
    public class BinancePriceService
    {
        public static async Task<decimal> GetTRXPriceUSDAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "TronAksaSharp/1.0");

            var response = await client.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=tron&vs_currencies=usd");

            if (!response.IsSuccessStatusCode)
                return 0;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("tron", out var tron) &&
                tron.TryGetProperty("usd", out var usdPrice))
            {
                return usdPrice.GetDecimal();
            }

            return 0;
        }

        public static async Task<decimal> GetTRXPriceTRYAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "TronAksaSharp/1.0");

            var response = await client.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=tron&vs_currencies=try");

            if (!response.IsSuccessStatusCode)
                return 0;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("tron", out var tron) &&
                tron.TryGetProperty("try", out var tryPrice))
            {
                return tryPrice.GetDecimal();
            }

            return 0;
        }
    }
}
