using TronAksaSharp.Endcoding;
using TronAksaSharp.Enums;
using TronAksaSharp.Services;
using TronAksaSharp.Wallet;

static void Step(string title)
{
    Console.WriteLine();
    Console.WriteLine($"════════════ {title} ════════════");
}

//-----------------------------------------------------------------------------TRONGRİD HESAP BİLGİLERİ

//-----------------------------------------------------------------------------TRONGRİD HESAP BİLGİLERİ


// TRON ADDRESS GENERATE :
Step("WALLET OLUŞTURMA");
string ToHex(byte[] data) => BitConverter.ToString(data).Replace("-", "");
var createWallet = TronClient.CreateTronWallet();
Console.WriteLine($"Cüzdan Adresi :\n {createWallet.Address}");
Console.WriteLine($"Private Key :\n {ToHex(createWallet.PrivateKey)}");
Console.WriteLine($"Public Key  :\n {ToHex(createWallet.PublicKey)}");


//-----------------------------------------------------------------------------


// ADDRESS BYTE UZUNLUĞU SORGULAMA :
Step("ADDRESS BİLGİSİ");
byte[] addrBytes = Base58.Decode($"{createWallet.Address}");
Console.WriteLine("Adres byte uzunluğu = " + addrBytes.Length);


//-----------------------------------------------------------------------------


// TRX BAKİYE VE STAKE SORGULAMA :
Step("BAKİYE & STAKE");
string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"; // Örnek TRON adresi

var balances = await TronClient.GetBalancesAsync(walletAddress, TronNetwork.NileTestNet);

Console.WriteLine($"TRX       : {balances.TrxBalance}");
Console.WriteLine($"Bandwidth : {balances.BandwidthStake}");
Console.WriteLine($"Energy    : {balances.EnergyStake}");



//-----------------------------------------------------------------------------

// TRC20 TOKEN BAKİYE SORGULAMA :
Step("TRC20 BAKİYE");
string trc20Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT TRC20 Contract Address
decimal usdtBalance = await TronClient.GetTRC20BalanceAsync(
    walletAddress,
    trc20Contract,
    6, // USDT decimals
    TronNetwork.NileTestNet
);

Console.WriteLine($"USDT Balance: {usdtBalance}");


//-----------------------------------------------------------------------------

// TRX TRANSFER TESTİ (TRX – Nile TestNet)

string senderAddress = "TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto";
string senderPrivateKey = "46599a45b3f178f6406f3b53f4b4f61f0cd9b6d4b2dab318c765d5d2fe78b1b9"; // Gönderen cüzdanın özel anahtarı 
string receiverAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

Step("TRX TRANSFER");
var trx = await TronClient.SendTRXAsync(
    senderAddress, 
    senderPrivateKey,
    receiverAddress, 
    10, 
    TronNetwork.NileTestNet);

var txInfo = await ManualTransactionInfoService.WaitForTransactionAsync(trx.TxId, TronNetwork.NileTestNet);


Console.WriteLine("----- TRX TRANSFER INFO -----");
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
    10,
    decimals,
    TronNetwork.NileTestNet
);

var trc20TxInfo = await ManualTransactionInfoService.WaitForTransactionAsync(
    trc20.TxId,
    TronNetwork.NileTestNet,
    decimals,
    usdtContract);



Console.WriteLine("----- TRC20 TRANSFER INFO -----");
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





