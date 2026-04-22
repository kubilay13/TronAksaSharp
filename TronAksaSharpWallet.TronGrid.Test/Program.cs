using TronAksaSharp.Endcoding;
using TronAksaSharp.Enums;
using TronAksaSharp.Wallet;

static void Step(string title)
{
    Console.WriteLine();
    Console.WriteLine($"════════════ {title} ════════════");
}

// ============================================================================
// WALLET OLUŞTURMA
// ============================================================================
Step("WALLET OLUŞTURMA");
var createWallet = TronClient.CreateTronWallet();
Console.WriteLine($"Cüzdan Adresi :\n {createWallet.Address}");
Console.WriteLine($"Private Key :\n {createWallet.PrivateKeyHex}");
Console.WriteLine($"Public Key  :\n {createWallet.PublicKeyHex}");

// ============================================================================
// ADRES BYTE UZUNLUĞU
// ============================================================================
Step("ADRES BYTE UZUNLUK SORGULAMA");
byte[] addrBytes = Base58.Decode($"{createWallet.Address}");
Console.WriteLine("Adres byte uzunluğu = " + addrBytes.Length);

// ============================================================================
// TRX BAKİYE VE STAKE
// ============================================================================
Step("BAKİYE & STAKE SORGULAMA");
string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

var balances = await TronClient.GetBalancesAsync(walletAddress, TronNetwork.NileTestNet);

Console.WriteLine($"TRX       : {balances.TrxBalance}");
Console.WriteLine($"Bandwidth : {balances.BandwidthStake}");
Console.WriteLine($"Energy    : {balances.EnergyStake}");

// ============================================================================
// TRC20 BAKİYE
// ============================================================================
Step("TRC20 BAKİYE SORGULAMA");
string trc20Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf";
decimal usdtBalance = await TronClient.GetTRC20BalanceAsync(
    walletAddress,
    trc20Contract,
    6,
    TronNetwork.NileTestNet
);
Console.WriteLine($"USDT Balance: {usdtBalance}");

// ============================================================================
// TRX TRANSFER
// ============================================================================
Step("TRX TRANSFER İŞLEMİ");

string senderAddress = "TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto";
string senderPrivateKey = "46599a45b3f178f6406f3b53f4b4f61f0cd9b6d4b2dab318c765d5d2fe78b1b9";
string receiverAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

var trx = await TronClient.SendTRXAsync(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    10,
    TronNetwork.NileTestNet
);

Console.WriteLine($"TRX Transfer: {trx.Success} - TxID: {trx.TxId}");

// ============================================================================
// TRC20 TRANSFER
// ============================================================================
Step("TRC20 TRANSFER İŞLEMİ");

var trc20Result = await TronClient.SendTRC20Async(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    trc20Contract,
    10,
    6,
    TronNetwork.NileTestNet
);

Console.WriteLine($"TRC20 Transfer: {trc20Result.Success} - TxID: {trc20Result.TxId}");