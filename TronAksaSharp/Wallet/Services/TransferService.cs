using System.Text;
using System.Text.Json;
using TronAksaSharp.Address;
using TronAksaSharp.Enums;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Wallet.Services
{
    public class TronTransferService
    {
        public static async Task<JsonDocument> CreateTRXTransactionAsync(
            string fromAddress,
            string toAddress,
            decimal amountTrx,
            TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            long amountSun = decimal.ToInt64(decimal.Round(amountTrx * 1_000_000m, 0, MidpointRounding.AwayFromZero));

            var payload = new
            {
                owner_address = AddressConverter.ToHex21(fromAddress),
                to_address = AddressConverter.ToHex21(toAddress),
                amount = amountSun
            };

            using var client = new HttpClient();
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                    $"{baseUrl}/wallet/createtransaction",
                    content);

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }

        public static async Task<bool> BroadcastAsync(
            JsonDocument tx,
            string signatureHex,
            TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(
                tx.RootElement.GetRawText());

            payload["signature"] = new[] { signatureHex };

            using var client = new HttpClient();
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                $"{baseUrl}/wallet/broadcasttransaction",
                content);

            var json = await response.Content.ReadAsStringAsync();
            return json.Contains("\"result\":true");
        }
    }
}
