using System.Text.Json;
using TronAksaSharp.Enums;
using TronAksaSharp.Networks;
using TronAksaSharp.Wallet;

static void Step(string title)
{
    Console.WriteLine();
    Console.WriteLine($"════════════ {title} ════════════");
}

// TRON ADDRESS GENERATE :
Step("WALLET OLUŞTURMA");

var createWallet = TronClient.CreateTronWallet();
Console.WriteLine($"Cüzdan Adresi :\n {createWallet.Address}");
Console.WriteLine($"Private Key :\n {createWallet.PrivateKeyHex}"); 
Console.WriteLine($"Public Key  :\n {createWallet.PublicKeyHex}");


//-----------------------------------------------------------------------------

// ADDRESS BYTE UZUNLUĞU SORGULAMA :
Step("ADRES BYTE UZUNLUK SORGULAMA");
int byteLength = TronClient.GetAddressByteLength(createWallet.Address);
Console.WriteLine("Adres byte uzunluğu = " + byteLength);


//-----------------------------------------------------------------------------
// TRX BAKİYE VE STAKE SORGULAMA :
Step("BAKİYE & STAKE MANUEL RPC ÇAĞRISI");
string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"; // Örnek TRON adresi

var balances = await TronClient.GetBalancesAsync(walletAddress, TronNetwork.NileTestNet);

Console.WriteLine($"TRX Miktarı : {balances.TrxBalance}");
Console.WriteLine($"Energy Miktarı: {balances.EnergyStake}");
Console.WriteLine($"Bandwidth Miktarı: {balances.BandwidthStake}");
Console.WriteLine($"Energy İçin Stake Edilen TRX  Miktarı: {balances.EnergyForTRXStake}");
Console.WriteLine($"Bandwidth İçim Stake Edilen TRX Miktarı : {balances.BandwidthForTRXStake}");


//-----------------------------------------------------------------------------

// TRC20 TOKEN BAKİYE SORGULAMA :
Step("TRC20 BAKİYE MANUEL RPC ÇAĞRISI");
string trc20Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT TRC20 Contract Address
decimal usdtBalance = await TronClient.GetTRC20BalanceAsync(
    walletAddress,
    trc20Contract,
    6, // USDT decimals
    TronNetwork.NileTestNet
);

Console.WriteLine($"USDT Balance: {usdtBalance}");



string baseUrl = TronEndpoints.GetBaseUrl(TronNetwork.NileTestNet);

using var client = new HttpClient();
var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{walletAddress}");
using var doc = JsonDocument.Parse(json);

if (!doc.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
{
    Console.WriteLine("Veri yok");
    return;
}

var acc = data[0];

Console.WriteLine("=== TÜM SAYISAL ALANLAR ===");
foreach (var prop in acc.EnumerateObject())
{
    if (prop.Value.ValueKind == JsonValueKind.Number)
    {
        Console.WriteLine($"{prop.Name}: {prop.Value.GetInt64()}");
    }
}

// account_resource içindekiler
if (acc.TryGetProperty("account_resource", out var res) && res.ValueKind == JsonValueKind.Object)
{
    Console.WriteLine("\n=== account_resource İÇİNDEKİLER ===");
    foreach (var prop in res.EnumerateObject())
    {
        if (prop.Value.ValueKind == JsonValueKind.Number)
        {
            Console.WriteLine($"{prop.Name}: {prop.Value.GetInt64()}");
        }
    }
}

decimal usdPrice = await TronClient.GetTRXPriceUSDAsync();
decimal tryPrice = await TronClient.GetTRXPriceTRYAsync();

Console.WriteLine($"TRX Fiyatı: ${usdPrice} USD");
Console.WriteLine($"TRX Fiyatı: {tryPrice} TL");