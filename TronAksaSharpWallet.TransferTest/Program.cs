using TronAksaSharp.Enums;
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

Console.WriteLine(trx.Success ? "TRX Transferi Başarılı" : $"TRX HATA → {trx.Error}");


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

Console.WriteLine(trc20.Success ? "TRC20 Transferi Başarılı" : $"TRC20 HATA → {trc20.Error}");

