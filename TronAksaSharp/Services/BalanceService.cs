using System.Numerics;
using System.Text;
using System.Text.Json;
using TronAksaSharp.Address;
using TronAksaSharp.Enums;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Services
{
    public class BalanceService
    {

        // TRX BAKİYESİNİ SORGULAR
        public static async Task<decimal> GetTRXBalanceAsync(string address ,TronNetwork tronNetwork)
        { 
           if(string.IsNullOrWhiteSpace(address))
           {
                throw new ArgumentException("Addres boş olamaz");
           }
           string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);

           using var client = new HttpClient();
           var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");

           var document = JsonDocument.Parse(json);
           if (!document.RootElement.TryGetProperty("data", out var data)  || data.GetArrayLength() == 0)
           { 
              return 0;
           }
           var balance = data[0].GetProperty("balance").GetDecimal();
           return balance / 1_000_000; // TRX birimi 1 TRX = 1,000,000 sunucudur.
        }
        // TRC20 BAKİYESİNİ SORGULAR
        public static async Task<decimal> GetTRC20BalanceAsync(string walletAddress, string contractAddress, int tokenDecimals, TronNetwork tronNetwork)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);

            using var client = new HttpClient();

            string hexParameterAddress = AddressConverter.ToHex32Parameter(walletAddress);

            var payload = new
            {
                contract_address = AddressConverter.ToHex21(contractAddress),
                function_selector = "balanceOf(address)",
                parameter = hexParameterAddress,
                owner_address = AddressConverter.ToHex21(walletAddress)
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/wallet/triggersmartcontract", content);

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("constant_result", out var result))
                return 0;

            string hexValue = result[0].GetString();

            var bigInt = BigInteger.Parse("0" + hexValue, System.Globalization.NumberStyles.HexNumber);

            return (decimal)bigInt / (decimal)Math.Pow(10, tokenDecimals);
        }
        // Bandwidth için stake edilmiş TRX miktarını döner
        public static async Task<decimal> GetStakedTRXForBandwidthAsync(string address, TronNetwork tronNetwork)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adres boş olamaz");

            string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);

            using var client = new HttpClient();
            var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                return 0;

            var acc = data[0];
            decimal bandwidthStake = 0;

            if (acc.TryGetProperty("frozenV2", out var frozenV2) && frozenV2.ValueKind == JsonValueKind.Array)
            {
                foreach (var f in frozenV2.EnumerateArray())
                {
                    if (f.TryGetProperty("amount", out var amount) && amount.ValueKind == JsonValueKind.Number)
                    {
                        if (!f.TryGetProperty("type", out var type) || type.GetString() == "BANDWIDTH")
                        {
                            bandwidthStake += amount.GetDecimal();
                        }
                    }
                }
            }

            return bandwidthStake / 1_000_000m;
        }
        // Energy için stake edilmiş TRX miktarını döner
        public static async Task<decimal> GetStakedEnergyForTRXAsync(string address, TronNetwork tronNetwork)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adres boş olamaz");

            string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);

            using var client = new HttpClient();
            var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                return 0;

            var acc = data[0];
            decimal energyStake = 0;

            if (acc.TryGetProperty("frozenV2", out var frozenV2) && frozenV2.ValueKind == JsonValueKind.Array)
            {
                foreach (var f in frozenV2.EnumerateArray())
                {
                    if (f.TryGetProperty("amount", out var amount) &&
                        f.ValueKind == JsonValueKind.Object &&
                        f.TryGetProperty("type", out var type) &&
                        type.GetString() == "ENERGY")
                    {
                        energyStake += amount.GetDecimal();
                    }
                }
            }

            if (acc.TryGetProperty("account_resource", out var res) && res.ValueKind == JsonValueKind.Object)
            {
                if (res.TryGetProperty("frozen_balance_for_energy", out var frozenEnergy) &&
                    frozenEnergy.ValueKind == JsonValueKind.Object)
                {
                    if (frozenEnergy.TryGetProperty("frozen_balance", out var energyBalance) &&
                        energyBalance.ValueKind == JsonValueKind.Number)
                    {
                        energyStake += energyBalance.GetDecimal();
                    }
                }
            }
            return energyStake / 1_000_000m;
        }
        // Bandwidth miktarını döner
        public static async Task<long> GetBandwidthAsync(string address, TronNetwork tronNetwork)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adres boş olamaz");

            string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);

            using var client = new HttpClient();
            var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                return 0;

            var acc = data[0];

            // net_window_size alanını kontrol et
            if (acc.TryGetProperty("net_window_size", out var netWindow) && netWindow.ValueKind == JsonValueKind.Number)
                return netWindow.GetInt64();

            return 0;
        }
        // Energy miktarını döner
        public static async Task<long> GetEnergyAsync(string address, TronNetwork tronNetwork)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);
            using var client = new HttpClient();
            var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                return 0;

            var acc = data[0];

            if (acc.TryGetProperty("energy_window_size", out var energyWindow) && energyWindow.ValueKind == JsonValueKind.Number)
                return energyWindow.GetInt64();

            return 0;
        }

        // ================= KENDİ STAKE KAYNAKLARI (account_resource) =================
        /// <summary>
        /// Kendi stake ettiğiniz TRX karşılığında elde ettiğiniz Bandwidth (byte) ve Energy (birim) miktarlarını döndürür.
        /// </summary>
        public static async Task<(long Bandwidth, long Energy)> GetSelfResourcesAsync(string address, TronNetwork tronNetwork)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adres boş olamaz");

            string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);
            using var client = new HttpClient();
            var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                return (0, 0);

            var acc = data[0];
            long bandwidth = 0, energy = 0;

            if (acc.TryGetProperty("account_resource", out var res) && res.ValueKind == JsonValueKind.Object)
            {
                if (res.TryGetProperty("net_limit", out var netLimit) && netLimit.ValueKind == JsonValueKind.Number)
                    bandwidth = netLimit.GetInt64();
                if (res.TryGetProperty("energy_limit", out var energyLimit) && energyLimit.ValueKind == JsonValueKind.Number)
                    energy = energyLimit.GetInt64();
            }

            return (bandwidth, energy);
        }

        // ================= DELEGASYON (BAŞKALARINDAN GELEN) KAYNAKLAR =================
        /// <summary>
        /// Başkaları tarafından size delegate edilen Bandwidth (byte) ve Energy (birim) miktarlarını döndürür.
        /// </summary>
        public static async Task<(long Bandwidth, long Energy)> GetDelegatedResourcesAsync(string address, TronNetwork tronNetwork)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adres boş olamaz");

            string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);
            using var client = new HttpClient();
            var response = await client.GetAsync($"{baseUrl}/v1/accounts/{address}/delegated");
            if (!response.IsSuccessStatusCode)
                return (0, 0);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            long bandwidth = 0, energy = 0;

            if (doc.RootElement.TryGetProperty("data", out var data) && data.GetArrayLength() > 0)
            {
                var item = data[0];
                if (item.TryGetProperty("bandwidth", out var bw) && bw.ValueKind == JsonValueKind.Number)
                    bandwidth = bw.GetInt64();
                if (item.TryGetProperty("energy", out var eng) && eng.ValueKind == JsonValueKind.Number)
                    energy = eng.GetInt64();
            }

            return (bandwidth, energy);
        }

        // ================= TOPLAM KULLANILABİLİR KAYNAKLAR =================
        /// <summary>
        /// Kendi stake + delegasyon ile elde edilen toplam kullanılabilir Bandwidth (byte) ve Energy (birim) miktarlarını döndürür.
        /// Bu değerler TronLink'te gördüğünüz değerlerle birebir aynı olmalıdır.
        /// </summary>
        public static async Task<(long TotalBandwidth, long TotalEnergy)> GetTotalResourcesAsync(string address, TronNetwork tronNetwork)
        {
            var (selfBandwidth, selfEnergy) = await GetSelfResourcesAsync(address, tronNetwork);
            var (delBandwidth, delEnergy) = await GetDelegatedResourcesAsync(address, tronNetwork);

            return (selfBandwidth + delBandwidth, selfEnergy + delEnergy);
        }

    }
}

