using System.Text.Json;
using TronAksaSharp.Enums;

namespace TronAksaSharp.Wallet
{
    public class BalanceService
    {
        /// <summary>
        /// TRX BAKİYESİNİ SORGULAR
        /// </summary>
        public static async Task<decimal> GetTRXBalanceAsync(string address ,TronNetwork tronNetwork)
        { 
           if(string.IsNullOrWhiteSpace(address))
           {
                throw new ArgumentException("Addres boş olamaz");
           }
           string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);

           using var client = new HttpClient();
           var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");

           var document = System.Text.Json.JsonDocument.Parse(json);
           if (!document.RootElement.TryGetProperty("data", out var data)  || data.GetArrayLength() == 0)
           { 
              return 0;
           }
           var balance = data[0].GetProperty("balance").GetDecimal();
           return balance / 1_000_000; // TRX birimi 1 TRX = 1,000,000 sunucudur.
        }

        /// <summary>
        /// Bandwidth için stake edilmiş TRX miktarını döner
        /// </summary>
        public static async Task<decimal> GetBandwidthStakeAsync(string address, TronNetwork tronNetwork)
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


        /// <summary>
        /// Energy için stake edilmiş TRX miktarını döner
        /// </summary>
        public static async Task<decimal> GetEnergyStakeAsync(string address, TronNetwork tronNetwork)
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

            // account_resource içindeki frozen_balance_for_energy'yi de ekle
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

    }
}

