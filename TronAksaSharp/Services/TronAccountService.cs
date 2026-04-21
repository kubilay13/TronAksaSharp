using System.Text;
using System.Text.Json;
using TronAksaSharp.Abstractions;
using TronAksaSharp.Address;
using TronAksaSharp.Enums;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Services
{
    public class TronAccountService : ITronAccountService
    {
        private readonly HttpClient _httpClient;

        public TronAccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JsonDocument> GetAccountAsync(string address, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            var payload = new
            {
                address = AddressConverter.ToHex21(address)

                // Tron Apisi gelen veriyi hex olarak ister bu yüzden bu yüzden gelen addressi hex21 formatına çeviriyoruz.
                // var payload = new metodu Anonim bir nesne oluşturur ve adresi içeriye atılarak hexe çevirir.
            };
          
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Encoding.UTF8 → Türkçe karakterler dahil her şey düzgün çalışsın diye UTF8 kullanıyor

            var response = await _httpClient.PostAsync($"{baseUrl}/wallet/getaccount", content);

            response.EnsureSuccessStatusCode();// Gelen yanıtı kontrol eder.

            var JsonString = await response.Content.ReadAsStringAsync(); // Cevabın içindeki JSON stringi okur.

            return JsonDocument.Parse(JsonString); // JSON stringini C# nesnesine çevirir ve buda veriyi okuyup içindeki bilgilere erişmemizi ve sorgulamamazı sağlar.
        }
    }
}
