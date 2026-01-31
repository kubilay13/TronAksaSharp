using System.Net.Http.Json;
using TronAksaSharp.Models.TronGrid.TronAccount;

namespace TronAksaSharp.Services
{
    public class TronGridService
    {
        private readonly HttpClient _httpClient;

        public TronGridService(string apikey)
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress= new Uri("https://nile.trongrid.io");

            _httpClient.DefaultRequestHeaders.Add("TRON-PRO-API-KEY",apikey);
        }

        public async Task<TronAccount?> GetAccountAsync(string address)
        {
            // GET /v1/accounts/{address}
            try
            {
                var response = await _httpClient.GetAsync($"/v1/accounts/{address}");

                response.EnsureSuccessStatusCode(); // HTTP hatalarında exception fırlatır

                // JSON'u doğrudan TronAccount sınıfına çevir
                var tronAccountWrapper = await response.Content.ReadFromJsonAsync<TronAccountWrapper>();

                return tronAccountWrapper?.Data?[0]; // JSON yapısında "data" dizisi var
            }
            catch (HttpRequestException ex)
            {
                // Hata loglama veya geri dönüş
                Console.WriteLine($"HTTP Error: {ex.Message}");
                return null;
            }
        }
    }
}
