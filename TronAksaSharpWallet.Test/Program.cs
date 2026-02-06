using TronAksaSharp.Endcoding;
using TronAksaSharp.Enums;
using TronAksaSharp.Services;
using TronAksaSharp.Wallet;

static void Step(string title)
{
    Console.WriteLine();
    Console.WriteLine($"════════════ {title} ════════════");
}

// TRON ADDRESS GENERATE :
Step("WALLET OLUŞTURMA");
string ToHex(byte[] data) => BitConverter.ToString(data).Replace("-", "");
var createWallet = TronClient.CreateTronWallet();
Console.WriteLine($"Cüzdan Adresi :\n {createWallet.Address}");
Console.WriteLine($"Private Key :\n {ToHex(createWallet.PrivateKey)}");
Console.WriteLine($"Public Key  :\n {ToHex(createWallet.PublicKey)}");


//-----------------------------------------------------------------------------

// ADDRESS BYTE UZUNLUĞU SORGULAMA :
Step("ADRES BYTE UZUNLUK SORGULAMA");
byte[] addrBytes = Base58.Decode($"{createWallet.Address}");
Console.WriteLine("Adres byte uzunluğu = " + addrBytes.Length);

//-----------------------------------------------------------------------------

// TRONGRİD HESAP BİLGİLERİ ÇEKME :
Step("ÖRNEK ADRES BİLGİLERİNİ TRONGRİD İLE SORGULAMA");
var service = new TronClient("TRONGRİD-APİ-KEY", TronNetwork.NileTestNet);
var accountdetail = await service.GetTronGridAccountDetailAsync("TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"); // Buraya sorgulamak istediğiniz TRON adresini yazabilirsiniz

if (accountdetail == null)
{
    Console.WriteLine("Account bulunamadı");
    return;
}

Console.WriteLine("====== TRON ACCOUNT ======");
Console.WriteLine($"Adres: {accountdetail.Address}");
Console.WriteLine($"Bakiye (TRX): {accountdetail.Balance / 1_000_000m}");
Console.WriteLine($"Oluşturulma: {DateTimeOffset.FromUnixTimeMilliseconds(accountdetail.CreateTime)}");
Console.WriteLine();

Console.WriteLine("====== FROZEN ======");
if (accountdetail.Frozen != null)
{
    foreach (var f in accountdetail.Frozen)
    {
        Console.WriteLine($"{f.Type}: {f.Amount / 1_000_000m} TRX");
    }
}
else
{
    Console.WriteLine("Frozen yok");
}
Console.WriteLine();

Console.WriteLine("====== ENERGY / RESOURCE ======");
if (accountdetail.AccountResource != null)
{
    Console.WriteLine($"Energy Optimized: {accountdetail.AccountResource.EnergyWindowOptimized}");
    Console.WriteLine($"Energy Window Size: {accountdetail.AccountResource.EnergyWindowSize}");
    Console.WriteLine($"Son Energy Harcama: {accountdetail.AccountResource.LatestConsumeTimeForEnergy}");
}
Console.WriteLine();

Console.WriteLine("====== PERMISSIONS ======");
Console.WriteLine("OWNER:");
Console.WriteLine($"Threshold: {accountdetail.OwnerPermission.Threshold}");
foreach (var key in accountdetail.OwnerPermission.Keys)
{
    Console.WriteLine($" - {key.Address} (Weight: {key.Weight})");
}

Console.WriteLine("ACTIVE:");
foreach (var perm in accountdetail.ActivePermissions)
{
    Console.WriteLine($"Permission: {perm.PermissionName}");
    Console.WriteLine($"Threshold: {perm.Threshold}");
    Console.WriteLine($"Operations: {perm.Operations}");
}
Console.WriteLine();

Console.WriteLine("====== VOTES ======");
if (accountdetail.Votes != null)
{
    foreach (var v in accountdetail.Votes)
    {
        Console.WriteLine($"SR: {v.VoteAddress} | Oy: {v.VoteCount}");
    }
}
Console.WriteLine();

Console.WriteLine("====== TRC20 TOKENS ======");
if (accountdetail.Trc20Tokens != null)
{
    foreach (var token in accountdetail.Trc20Tokens)
    {
        foreach (var kv in token)
        {
            Console.WriteLine($"Token: {kv.Key}");
            Console.WriteLine($"Balance (raw): {kv.Value}");
        }
    }
}
else
{
    Console.WriteLine("TRC20 yok");
}

Console.WriteLine("====== BİTTİ ======");

//-----------------------------------------------------------------------------


// TRX BAKİYE VE STAKE SORGULAMA :
Step("BAKİYE & STAKE MANUEL RPC ÇAĞRISI");
string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"; // Örnek TRON adresi

var balances = await TronClient.GetBalancesAsync(walletAddress, TronNetwork.NileTestNet);

Console.WriteLine($"TRX       : {balances.TrxBalance}");
Console.WriteLine($"Bandwidth : {balances.BandwidthStake}");
Console.WriteLine($"Energy    : {balances.EnergyStake}");



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


//-----------------------------------------------------------------------------

// TRX TRANSFER TESTİ (TRX – Nile TestNet)

string senderAddress = "TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto";
string senderPrivateKey = "46599a45b3f178f6406f3b53f4b4f61f0cd9b6d4b2dab318c765d5d2fe78b1b9"; // Gönderen cüzdanın özel anahtarı 
string receiverAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

Step("TRX TRANSFER İŞLEMİ NİLE ");
var trx = await TronClient.SendTRXAsync(
    senderAddress, 
    senderPrivateKey,
    receiverAddress, 
    10, 
    TronNetwork.NileTestNet);

var txInfo = await ManualTransactionInfoService.WaitForTransactionAsync(trx.TxId, TronNetwork.NileTestNet);


Console.WriteLine("----- TRX TRANSFER INFO RPC MANUEL ÇAĞRISI -----");
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
Step("TRC20 TRANSFER İŞLEMİ NİLE ");
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



Console.WriteLine("----- TRC20 TRANSFER INFO RPC MANUEL ÇAĞRISI -----");
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
Console.WriteLine("TRC20 Transferi Başarılı Şekilde Gönderildi.");




