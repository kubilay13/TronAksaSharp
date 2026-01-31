using System.Numerics;
using System.Text;
using System.Text.Json;
using TronAksaSharp.Address;
using TronAksaSharp.Enums;
using TronAksaSharp.Models.TronTransaction;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Services
{
    public class ManualTransactionInfoService
    {
        public static async Task<TransactionInfo> WaitForTransactionAsync(string txId, TronNetwork network, int? trc20Decimals = null, string trc20ContractAddress = null)
        {
            while (true)
            {
                //  gettransactionbyid ile raw bilgiyi al
                var rawDoc = await GetTransactionByIdAsync(txId, network);
                var root = rawDoc.RootElement;

                long timestamp = root.GetProperty("raw_data").GetProperty("timestamp").GetInt64();
                string from = string.Empty;
                string to = string.Empty;
                decimal amount = 0;
                string asset = string.Empty;

                if (root.GetProperty("raw_data").TryGetProperty("contract", out var contracts))
                {
                    foreach (var contract in contracts.EnumerateArray())
                    {
                        var type = contract.GetProperty("type").GetString();

                        // TRX işlemi
                        if (type == "TransferContract" && trc20Decimals == null)
                        {
                            var value = contract.GetProperty("parameter").GetProperty("value");
                            string fromHex = value.GetProperty("owner_address").GetString();
                            string toHex = value.GetProperty("to_address").GetString();
                            long amountSun = value.GetProperty("amount").GetInt64();

                            from = AddressConverter.HexToBase58(fromHex);
                            to = AddressConverter.HexToBase58(toHex);
                            amount = amountSun / 1_000_000m;
                            asset = "TRX";
                            break;
                        }

                        // TRC20 işlemi
                        if (type == "TriggerSmartContract" && trc20Decimals.HasValue)
                        {
                            var value = contract.GetProperty("parameter").GetProperty("value");
                            string contractHex = value.GetProperty("contract_address").GetString();
                            if (!contractHex.Equals(AddressConverter.ToHex21(trc20ContractAddress!), StringComparison.OrdinalIgnoreCase))
                                continue;

                            string ownerHex = value.GetProperty("owner_address").GetString();
                            string dataHex = value.GetProperty("data").GetString();

                            from = AddressConverter.HexToBase58(ownerHex);

                            string toHex = "41" + dataHex.Substring(8 + 24, 40);
                            to = AddressConverter.HexToBase58(toHex);

                            BigInteger amountBI = BigInteger.Parse(dataHex.Substring(8 + 64, 64), System.Globalization.NumberStyles.HexNumber);
                            amount = (decimal)amountBI / (decimal)Math.Pow(10, trc20Decimals.Value);

                            asset = $"TRC20:{trc20ContractAddress}";
                            break;
                        }
                    }
                }

                // gettransactioninfobyid ile blok numarası ve fee bilgilerini al
                var info = await GetTransactionInfoAsync(txId, network);
                long blockNumber = info.BlockNumber;

                // İşlem bloğa dahil olmuşsa dön
                if (blockNumber > 0 && !string.IsNullOrEmpty(from))
                {
                    return new TransactionInfo
                    {
                        TxId = txId,
                        BlockNumber = blockNumber,
                        From = from,
                        To = to,
                        Amount = amount,
                        Asset = asset,
                        Timestamp = timestamp,
                        Fee = info.Fee,
                        EnergyUsed = info.EnergyUsed,
                        NetFee = info.NetFee,
                        Result = info.Result
                    };
                }
                // Henüz blokta değilse 3 saniye bekle
                await Task.Delay(3000);
            }
        }

        private static async Task<JsonDocument> GetTransactionByIdAsync(string txId, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);
            var payload = new { value = txId };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/wallet/gettransactionbyid", content);
            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }

        public static async Task<TransactionInfo> GetTransactionInfoAsync(string txId, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);
            var payload = new { value = txId };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/wallet/gettransactioninfobyid", content);
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;
            if (!root.TryGetProperty("blockNumber", out var blockProp))
                return new TransactionInfo { TxId = txId, BlockNumber = 0 };

            long fee = 0, energy = 0, netFee = 0;
            string result = "Successful";

            if (root.TryGetProperty("fee", out var feeProp))
                fee = feeProp.GetInt64();
            if (root.TryGetProperty("result", out var resultProp))
                result = resultProp.GetString();
            if (root.TryGetProperty("receipt", out var receipt))
            {
                if (receipt.TryGetProperty("energy_usage_total", out var e))
                    energy = e.GetInt64();
                if (receipt.TryGetProperty("net_fee", out var n))
                    netFee = n.GetInt64();
            }

            return new TransactionInfo
            {
                TxId = txId,
                BlockNumber = blockProp.GetInt64(),
                Fee = fee,
                EnergyUsed = energy,
                NetFee = netFee,
                Result = result
            };
        }
    }
}
