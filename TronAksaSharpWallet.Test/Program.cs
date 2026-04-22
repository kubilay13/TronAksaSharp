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




