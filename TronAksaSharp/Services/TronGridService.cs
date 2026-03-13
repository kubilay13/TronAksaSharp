using System.Net.Http.Json;
using System.Text.Json;
using TronAksaSharp.Enums;
using TronAksaSharp.Models.TronGrid;
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

        // Belirtilen TRON adresine ait TRX işlemleri döner (limit parametresi ile sonuç sayısı sınırlandırılabilir TRONGRİD MAX SINIR 200 SONRA HATA DÖNER APİKEY BAN YİYEBİLİRSİNİZ.)
        public async Task<List<TronTransaction>> GetTRXTransactiondDetailsAsync(string address, int? limit = null)
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
        // Belirtilen TRON adresine ait TRC20 işlemleri döner (limit parametresi ile sonuç sayısı sınırlandırılabilir TRONGRİD MAX SINIR 200 SONRA HATA DÖNER
        public async Task<List<Trc20Transaction>> GetTRC20TransactionDetailsAsync(string address, int? limit = null)
        {
            var url = $"/v1/accounts/{address}/transactions/trc20";
            if (limit.HasValue)
                url += $"?limit={limit.Value}";

            var res = await _httpClient.GetAsync(url);
            res.EnsureSuccessStatusCode();

            var wrapper = await res.Content.ReadFromJsonAsync<Trc20TransactionWrapper>();
            var txList = wrapper?.Data ?? new List<Trc20Transaction>();

            foreach (var tx in txList)
            {
                try
                {
                    // Transaction detayları için doğru endpoint: /v1/transactions/{txId}/events?only_confirmed=true
                    var detailRes = await _httpClient.GetAsync($"/v1/transactions/{tx.TransactionId}/events?only_confirmed=true");

                    if (detailRes.IsSuccessStatusCode)
                    {
                        var detailJson = await detailRes.Content.ReadAsStringAsync();
                        var detailData = JsonDocument.Parse(detailJson);

                        // Root element "data" array'i içerir
                        if (detailData.RootElement.TryGetProperty("data", out var events) && events.GetArrayLength() > 0)
                        {
                            // İlk event'ten timestamp ve status bilgisini al
                            var firstEvent = events[0];

                            // Timestamp (block_timestamp olarak gelir)
                            if (firstEvent.TryGetProperty("block_timestamp", out var timestampProp))
                            {
                                var timestampMs = timestampProp.GetInt64();
                                tx.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).DateTime;
                            }

                            // Status - eğer event başarılıysa transaction da başarılıdır
                            tx.Status = "SUCCESS"; // Events endpoint'inden status gelmez, varsayılan olarak SUCCESS
                        }
                    }

                    // TransactionInfo endpoint'inden fee bilgisini al
                    // Fee bilgisi için: /v1/transactions/{txId}/info
                    var feeRes = await _httpClient.GetAsync($"/v1/transactions/{tx.TransactionId}/info");

                    if (feeRes.IsSuccessStatusCode)
                    {
                        var feeJson = await feeRes.Content.ReadAsStringAsync();
                        var feeData = JsonDocument.Parse(feeJson);

                        if (feeData.RootElement.TryGetProperty("fee", out var feeProp))
                        {
                            tx.Fee = feeProp.GetInt64() / 1_000_000m; // TRX formatına çevir
                        }
                    }

                    // Eğer timestamp hala boşsa, transaction wrapper'daki timestamp'i dene
                    if (tx.Timestamp == DateTime.MinValue)
                    {
                        // Transaction wrapper'da timestamp yoksa, mevcut zamanı kullan (fallback)
                        tx.Timestamp = DateTime.UtcNow;
                        tx.Status = tx.Status ?? "UNKNOWN";
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunda varsayılan değerler
                    tx.Status = "UNKNOWN";
                    tx.Fee = 0;
                    tx.Timestamp = DateTime.UtcNow;

                    // Hata loglaması yapabilirsiniz
                    Console.WriteLine($"Error fetching details for tx {tx.TransactionId}: {ex.Message}");
                }
            }

            return txList;
        }

        // TRON ağının anlık fee bilgilerini alır
        public async Task<FeeParameters> GetTRXFeeParametersAsync()
        {
            var response = await _httpClient.GetAsync("/wallet/getchainparameters");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var feeParams = new FeeParameters();
            if (doc.RootElement.TryGetProperty("chainParameter", out var parameters))
            {
                foreach (var param in parameters.EnumerateArray())
                {
                    if (param.TryGetProperty("key", out var key) &&
                        param.TryGetProperty("value", out var value))
                    {
                        string keyStr = key.GetString();
                        long valueLong = value.GetInt64();
                        decimal valueTrx = valueLong / 1_000_000m; // SUN -> TRX

                        if (keyStr == "getTransactionFee")
                        {
                            feeParams.TransactionFee = valueTrx;
                        }
                        else if (keyStr == "getEnergyFee")
                        {
                            feeParams.EnergyFee = valueTrx;
                        }
                    }
                }
            }
            if (feeParams.TransactionFee == 0)
                feeParams.TransactionFee = 0.001m; // 1000 SUN

            if (feeParams.EnergyFee == 0)
                feeParams.EnergyFee = 0.00004m; // 40 SUN

            return feeParams;
        }
    }
}
