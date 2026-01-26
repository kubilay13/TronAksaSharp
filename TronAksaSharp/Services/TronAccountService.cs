using System.Text;
using System.Text.Json;
using TronAksaSharp.Address;
using TronAksaSharp.Enums;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Services
{
    public class TronAccountService
    {
        public static async Task<JsonDocument> GetAccountAsync(string address, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            var payload = new
            {
                address = AddressConverter.ToHex21(address)
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/wallet/getaccount", content);

            return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        }
    }
}
