using System.Numerics;
using System.Text;
using System.Text.Json;
using TronAksaSharp.Address;
using TronAksaSharp.Crypto;
using TronAksaSharp.Enums;
using TronAksaSharp.Models;
using TronAksaSharp.Networks;

namespace TronAksaSharp.Services
{
    public class TronTransferService
    {
        public static async Task<JsonDocument> CreateTRXTransactionAsync(string fromAddress, string toAddress, decimal amountTrx, byte[] privateKey, TronNetwork network)
        {
            if (amountTrx <= 0)
                throw new ArgumentException("Gönderilecek TRX 0'dan büyük olmalı");

            string baseUrl = TronEndpoints.GetBaseUrl(network);

            long amountSun = decimal.ToInt64(decimal.Round(amountTrx * 1_000_000m, 0, MidpointRounding.AwayFromZero));

            if (amountSun <= 0)
                throw new Exception("Amount SUN'a çevrildiğinde 0 oldu");

            string signerAddress = TronAccountPermissionResolver.AddressFromPrivateKey(privateKey);

            var account = await TronAccountService.GetAccountAsync(fromAddress, network);

            int permissionId = TronAccountPermissionResolver.ResolvePermissionId(account, signerAddress);

            Console.WriteLine("FROM    = " + fromAddress);

            signerAddress = TronAccountPermissionResolver.AddressFromPrivateKey(privateKey);

            Console.WriteLine("SIGNER  = " + signerAddress);

             account = await TronAccountService.GetAccountAsync(fromAddress, network);

            permissionId = TronAccountPermissionResolver.ResolvePermissionId(account, signerAddress);

            Console.WriteLine("PERM_ID = " + permissionId);

            var payload = new
            {
                owner_address = AddressConverter.ToHex21(fromAddress),
                to_address = AddressConverter.ToHex21(toAddress),
                amount = amountSun,
                permission_id = permissionId
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/wallet/createtransaction", content);

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("Error", out var error))
                throw new Exception("TRX FAILED: " + error.GetString());

            if (!doc.RootElement.TryGetProperty("raw_data_hex", out _))
                throw new Exception("TRX FAILED: raw_data_hex yok");

            return doc;
        }
        public static async Task<TransferResult> BroadcastAsync(JsonElement tx, string signatureHex, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            using var txDoc = JsonDocument.Parse(tx.GetRawText());
            var root = txDoc.RootElement;

            var payload = new Dictionary<string, object>();

            foreach (var prop in root.EnumerateObject())
            {
                payload[prop.Name] = prop.Value;
            }

            payload["signature"] = new[] { signatureHex };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                $"{baseUrl}/wallet/broadcasttransaction", content

            );

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("result", out var result) ||
                result.ValueKind != JsonValueKind.True)
            {
                string? error = null;

                if (doc.RootElement.TryGetProperty("message", out var msg))
                {
                    error = Encoding.UTF8.GetString(
                        Convert.FromHexString(msg.GetString()));
                }

                return new TransferResult
                {
                    Success = false,
                    Error = error ?? json
                };
            }

            string txId = doc.RootElement.GetProperty("txid").GetString();

            return new TransferResult
            {
                Success = true,
                TxId = txId
            };
        }
        public static async Task<JsonDocument> CreateTRC20TransactionAsync(string fromAddress, string toAddress, string contractAddress, decimal amount, int decimals, int permissionId, TronNetwork network)
        {
            string baseUrl = TronEndpoints.GetBaseUrl(network);

            BigInteger tokenAmount = new BigInteger(decimal.Round(amount * (decimal)Math.Pow(10, decimals), 0, MidpointRounding.AwayFromZero));

            string parameter = AddressConverter.ToHex32Parameter(toAddress) + tokenAmount.ToString("x").PadLeft(64, '0');

            var payload = new
            {
                owner_address = AddressConverter.ToHex21(fromAddress),
                contract_address = AddressConverter.ToHex21(contractAddress),
                function_selector = "transfer(address,uint256)",
                parameter,
                fee_limit = 100_000_000,
                permission_id = permissionId   
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/wallet/triggersmartcontract", content);

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.GetProperty("result").GetProperty("result").GetBoolean())
            {
                string error = doc.RootElement.GetProperty("result").GetProperty("message").GetString();
                throw new Exception("TRC20 FAILED: " + error);
            }
            return doc;
        }
    }
}
