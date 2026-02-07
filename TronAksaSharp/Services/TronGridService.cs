using System.Net.Http.Json;
using TronAksaSharp.Enums;
using TronAksaSharp.Models.TronGrid.TronAccount;
using TronAksaSharp.Models.TronGrid.TronTransaction;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Services
{
    public class TronGridService
    {
        private readonly HttpClient _httpClient;
        public TronGridService(string apiKey, TronNetwork tronNetwork)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(TronEndpoints.GetBaseUrl(tronNetwork))
            };

            _httpClient.DefaultRequestHeaders.Add("TRON-PRO-API-KEY", apiKey);
        }

        // Belirtilen TRON adresinin tüm cüzdan bilgilerini döner
        public async Task<TronAccount?> GetAccountAsync(string address)
        {
            var response = await _httpClient.GetAsync($"/v1/accounts/{address}");
            response.EnsureSuccessStatusCode();

            var wrapper = await response.Content.ReadFromJsonAsync<TronAccountWrapper>();

            return wrapper?.Data?.FirstOrDefault();
        }

        // Belirtilen TRON adresine ait işlemleri döner (limit parametresi ile sonuç sayısı sınırlandırılabilir TRONGRİD MAX SINIR 200 SONRA HATA DÖNER APİKEY BAN YİYEBİLİRSİNİZ.)
        public async Task<List<TronTransaction>> GetTransactiondDetailsAsync(string address, int? limit = null)
        {
            var url = $"/v1/accounts/{address}/transactions";

            if (limit.HasValue)
            {
                url += $"?limit={limit.Value}";
            }         

            var res = await _httpClient.GetAsync(url);
            res.EnsureSuccessStatusCode();

            var wrapper = await res.Content.ReadFromJsonAsync<TronTransactionWrapper>();

            return wrapper?.Data ?? new List<TronTransaction>();
        }
    }
}
