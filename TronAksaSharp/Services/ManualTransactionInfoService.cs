using System.Numerics;
using System.Text;
using System.Text.Json;
using TronAksaSharp.Address;
using TronAksaSharp.Enums;
using TronAksaSharp.Models;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Services
{
    public class ManualTransactionInfoService
    {
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
            {
                return new TransactionInfo
                {
                    TxId = txId,
                    BlockNumber = 0
                };
            }

            long fee = 0;
            long energy = 0;
            long netFee = 0;
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

            var raw = await GetTransactionRawAsync(txId, network);

            return new TransactionInfo
            {
                TxId = txId,
                BlockNumber = blockProp.GetInt64(),
                Fee = fee,
                EnergyUsed = energy,
                NetFee = netFee,
                Result = result,
                From = raw.From,
                To = raw.To,
                Amount = raw.Amount,
                Asset = raw.Asset,
                Timestamp = raw.Timestamp
            };
        }

        public static async Task<TransactionInfo> WaitForConfirmationAsync(string txId, TronNetwork network)
        {
            while (true)
            {
                var info = await GetTransactionInfoAsync(txId, network);

                if (info.BlockNumber > 0)
                    return info;

                await Task.Delay(3000);
            }
        }


        // RAW TX çekme (from, to, amount, asset, timestamp)
        public static async Task<(string From, string To, decimal Amount, string Asset, long Timestamp)> GetTransactionRawAsync(string txId, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            var payload = new { value = txId };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/wallet/gettransactionbyid", content);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            long timestamp = root.GetProperty("raw_data").GetProperty("timestamp").GetInt64();

            var contract = root.GetProperty("raw_data").GetProperty("contract")[0];

            var type = contract.GetProperty("type").GetString();
            var value = contract.GetProperty("parameter").GetProperty("value");

            if (type == "TransferContract")
            {
                string fromHex = value.GetProperty("owner_address").GetString();
                string toHex = value.GetProperty("to_address").GetString();
                long amountSun = value.GetProperty("amount").GetInt64();

                string from = AddressConverter.HexToBase58(fromHex);
                string to = AddressConverter.HexToBase58(toHex);

                return (from, to, amountSun / 1_000_000m, "TRX", timestamp);
            }

            throw new Exception("Unsupported tx type");
        }
        public static async Task<TransactionInfo> GetTransactionInfoTRC20Async(string txId, TronNetwork network, int trc20Decimals, string trc20ContractAddress)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            var payload = new { value = txId };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/wallet/gettransactionbyid", content);
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            if (!root.TryGetProperty("blockNumber", out var blockProp))
            {
                return new TransactionInfo
                {
                    TxId = txId,
                    BlockNumber = 0
                };
            }

            long fee = 0;
            long energy = 0;
            long netFee = 0;
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

            // RAW TX bilgisi TRC20 için
            var raw = await GetTransactionRawTRC20Async(txId, network, trc20Decimals, trc20ContractAddress);

            return new TransactionInfo
            {
                TxId = txId,
                BlockNumber = blockProp.GetInt64(),
                Fee = fee,
                EnergyUsed = energy,
                NetFee = netFee,
                Result = result,
                From = raw.From,
                To = raw.To,
                Amount = raw.Amount,
                Asset = raw.Asset,
                Timestamp = raw.Timestamp
            };
        }

        public static async Task<(string From, string To, decimal Amount, string Asset, long Timestamp)> GetTransactionRawTRC20Async(
     string txId, TronNetwork network, int decimals, string contractAddress)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);
            var payload = new { value = txId };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/wallet/gettransactionbyid", content);
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;
            long timestamp = root.GetProperty("raw_data").GetProperty("timestamp").GetInt64();

            string from = string.Empty;
            string to = string.Empty;
            decimal amount = 0;
            string asset = string.Empty;

            if (root.GetProperty("raw_data").TryGetProperty("contract", out var contractArray))
            {
                foreach (var contract in contractArray.EnumerateArray())
                {
                    if (contract.GetProperty("type").GetString() != "TriggerSmartContract") continue;

                    var valueProp = contract.GetProperty("parameter").GetProperty("value");
                    string contractHex = valueProp.GetProperty("contract_address").GetString();

                    if (!contractHex.Equals(AddressConverter.ToHex21(contractAddress), StringComparison.OrdinalIgnoreCase))
                        continue;

                    string ownerHex = valueProp.GetProperty("owner_address").GetString();
                    string dataHex = valueProp.GetProperty("data").GetString();

                    from = AddressConverter.HexToBase58(ownerHex);
                    to = AddressConverter.HexToBase58("41" + dataHex.Substring(8, 64));

                    BigInteger amountBI = BigInteger.Parse(dataHex.Substring(8 + 64, 64), System.Globalization.NumberStyles.HexNumber);
                    amount = (decimal)amountBI / (decimal)Math.Pow(10, decimals);

                    asset = $"TRC20:{contractAddress}";
                    break;
                }
            }
            return (from, to, amount, asset, timestamp);
        }



        public static async Task<TransactionInfo> WaitForTRC20TransactionFixedAsync(string txId, TronNetwork network, int decimals, string contractAddress)
        {
            while (true)
            {
                var raw = await GetTransactionRawTRC20Async(txId, network, decimals, contractAddress);

                long blockNumber = 0;
                using (var infoDoc = await GetTransactionInfoByIdAsync(txId, network))
                {
                    var root = infoDoc.RootElement;
                    if (root.TryGetProperty("blockNumber", out var blockProp))
                        blockNumber = blockProp.GetInt64();
                }

                Console.WriteLine($"[WAIT] TxId={txId}, BlockNumber={blockNumber}, From={raw.From}, To={raw.To}, Amount={raw.Amount}, Asset={raw.Asset}");

                if (blockNumber > 0 && !string.IsNullOrEmpty(raw.From))
                {
                    return new TransactionInfo
                    {
                        TxId = txId,
                        BlockNumber = blockNumber,
                        From = raw.From,
                        To = raw.To,
                        Amount = raw.Amount,
                        Asset = raw.Asset,
                        Timestamp = raw.Timestamp
                    };
                }

                await Task.Delay(3000);
            }
        }
        private static async Task<JsonDocument> GetTransactionInfoByIdAsync(string txId, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);
            var payload = new { value = txId };
            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/wallet/gettransactionbyid", content);
            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }
    }
}
