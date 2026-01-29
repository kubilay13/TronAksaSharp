using TronAksaSharp.Enums;
using TronAksaSharp.Services;
using TronAksaSharp.Wallet;

static void Step(string title)
{
    Console.WriteLine();
    Console.WriteLine($"════════════ {title} ════════════");
}

//-----------------------------------------------------------------------------

// Örnek cüzdan bilgileri (Nile TestNet)

string senderAddress = "TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto";
string senderPrivateKey = "46599a45b3f178f6406f3b53f4b4f61f0cd9b6d4b2dab318c765d5d2fe78b1b9"; // Gönderen cüzdanın özel anahtarı 
string receiverAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

//-----------------------------------------------------------------------------

// TRX TRANSFER TESTİ (TRX – Nile TestNet)
Step("TRX TRANSFER");
var trx = await TronClient.SendTRXAsync(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    1,
    TronNetwork.NileTestNet
);

//Console.WriteLine(trx.Success ? "TRX Transferi Başarılı" : $"TRX HATA → {trx.Error}");
var txInfo = await ManualTransactionInfoService.WaitForConfirmationAsync(trx.TxId, TronNetwork.NileTestNet); // TRON NODE DENGESİZLİKLERİNDEN DOLAYI BAZEN HATA
                                                                                                             // VEREBİLİR BU YÜZDEN TRON GRİD KULLANMANIZ TAVSİYE EDİLİR.

Console.WriteLine("----- TX INFO -----");
Console.WriteLine("Block Number : " + txInfo.BlockNumber);
Console.WriteLine("Txn Hash : " + txInfo.TxId);
Console.WriteLine("Timestamp    : " + DateTimeOffset.FromUnixTimeMilliseconds(txInfo.Timestamp).UtcDateTime);
Console.WriteLine("From        : " + txInfo.From);
Console.WriteLine("To          : " + txInfo.To);
Console.WriteLine("Amount      : " + txInfo.Amount + " " + txInfo.Asset);
Console.WriteLine("Result      : " + txInfo.Result);
Console.WriteLine("Fee (SUN)   : " + txInfo.Fee);
Console.WriteLine("Energy Used : " + txInfo.EnergyUsed);
Console.WriteLine("Net Fee     : " + txInfo.NetFee);
Console.WriteLine("-------------------");
Console.WriteLine("TRX Transferi Başarılı Şekilde Gönderildi.");

//-----------------------------------------------------------------------------
// TRC20 TRANSFER TESTİ (USDT – Nile TestNet)
Step("TRC20 TRANSFER");
string usdtContract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT (Nile)
int decimals = 6; // USDT için ondalık basamak sayısı

var trc20 = await TronClient.SendTRC20Async(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    usdtContract,
    1,
    decimals,
    TronNetwork.NileTestNet
);

//Console.WriteLine(trc20.Success ? "TRC20 Transferi Başarılı" : $"TRC20 HATA → {trc20.Error}");

var trc20TxInfo = await ManualTransactionInfoService.WaitForTRC20TransactionFixedAsync(trc20.TxId, TronNetwork.NileTestNet, decimals, usdtContract); // TRON NODE DENGESİZLİKLERİNDEN DOLAYI BAZEN HATA
                                                                                                                                                     // VEREBİLİR BU YÜZDEN TRON GRİD KULLANMANIZ TAVSİYE EDİLİR.


Console.WriteLine("----- TRC20 TX INFO -----");
Console.WriteLine("Block Number : " + trc20TxInfo.BlockNumber);
Console.WriteLine("Txn Hash     : " + trc20TxInfo.TxId);
Console.WriteLine("Timestamp    : " + DateTimeOffset.FromUnixTimeMilliseconds(trc20TxInfo.Timestamp).UtcDateTime);
Console.WriteLine("From         : " + trc20TxInfo.From);
Console.WriteLine("To           : " + trc20TxInfo.To);
Console.WriteLine("Amount       : " + trc20TxInfo.Amount + " " + trc20TxInfo.Asset);
Console.WriteLine("Result       : " + trc20TxInfo.Result);
Console.WriteLine("Fee (SUN)    : " + trc20TxInfo.Fee);
Console.WriteLine("Energy Used  : " + trc20TxInfo.EnergyUsed);
Console.WriteLine("Net Fee      : " + trc20TxInfo.NetFee);
Console.WriteLine("--------------------------");
