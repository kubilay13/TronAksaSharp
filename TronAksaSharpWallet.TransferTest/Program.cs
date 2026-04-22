using TronAksaSharp.Enums;
using TronAksaSharp.Services;
using TronAksaSharp.Wallet;

static void Step(string title)
{
    Console.WriteLine();
    Console.WriteLine($"════════════ {title} ════════════");
}

// ============================================================================
// TEST VERİLERİ
// ============================================================================

string senderAddress = "TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto";
string senderPrivateKey = "46599a45b3f178f6406f3b53f4b4f61f0cd9b6d4b2dab318c765d5d2fe78b1b9";
string receiverAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

// ============================================================================
// TRX TRANSFER TESTİ
// ============================================================================

Step("TRX TRANSFER İŞLEMİ NİLE");

var trx = await TronClient.SendTRXAsync(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    10,
    TronNetwork.NileTestNet
);

var txInfo = await ManualTransactionInfoService.WaitForTransactionAsync(trx.TxId, TronNetwork.NileTestNet);

Console.WriteLine("----- TRX TRANSFER INFO -----");
Console.WriteLine($"TxID       : {txInfo.TxId}");
Console.WriteLine($"From       : {txInfo.From}");
Console.WriteLine($"To         : {txInfo.To}");
Console.WriteLine($"Amount     : {txInfo.Amount} TRX");
Console.WriteLine($"Result     : {txInfo.Result}");
Console.WriteLine("TRX Transferi Başarılı!");

// ============================================================================
// TRC20 TRANSFER TESTİ (USDT - Nile)
// ============================================================================

Step("TRC20 TRANSFER İŞLEMİ NİLE");

string usdtContract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf";
int decimals = 6;

var trc20 = await TronClient.SendTRC20Async(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    usdtContract,
    10,
    decimals,
    TronNetwork.NileTestNet
);

var trc20TxInfo = await ManualTransactionInfoService.WaitForTransactionAsync(
    trc20.TxId,
    TronNetwork.NileTestNet,
    decimals,
    usdtContract
);

Console.WriteLine("----- TRC20 TRANSFER INFO -----");
Console.WriteLine($"TxID       : {trc20TxInfo.TxId}");
Console.WriteLine($"From       : {trc20TxInfo.From}");
Console.WriteLine($"To         : {trc20TxInfo.To}");
Console.WriteLine($"Amount     : {trc20TxInfo.Amount} USDT");
Console.WriteLine($"Result     : {trc20TxInfo.Result}");
Console.WriteLine("TRC20 Transferi Başarılı!");