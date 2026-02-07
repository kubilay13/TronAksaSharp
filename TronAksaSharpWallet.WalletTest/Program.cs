using TronAksaSharp.Endcoding;
using TronAksaSharp.Enums;
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
Step("ADDRESS BİLGİSİ");
byte[] addrBytes = Base58.Decode($"{createWallet.Address}");
Console.WriteLine("Adres byte uzunluğu = " + addrBytes.Length);

//-----------------------------------------------------------------------------

// TRONGRİD HESAP BİLGİLERİ ÇEKME :
Step("ÖRNEK ADRES BİLGİLERİNİ TRONGRİD İLE SORGULAMA");
var service = new TronClient("TRONGRİD-APİ-KEY", TronNetwork.NileTestNet); // Buraya kendi TronGrid API anahtarınızı yazmalısınız
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

//-----------------------------------------------------------------------------
//TRONGRİD TRX İŞLEM DETAYLARI ÇEKME :

Step("TRONGRİD İLE İŞLEM DETAYLARI ÇEKME");

var txservice = new TronClient(
    apiKey: "", // Buraya kendi TronGrid API anahtarınızı yazmalısınız
    tronNetwork: TronNetwork.NileTestNet // Buraya sorgulamak istediğiniz TRON ağı (MainNet, NileTestNet, ShastaTestNet)
);

var txs = await txservice.GetTronGridTransactionDetailsAsync(
    address: "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", // Buraya sorgulamak istediğiniz TRON adresini yazabilirsiniz
    limit: 200 // Çekmek istediğiniz işlem sayısı (max 200) TRONGRİD SINIRI 
);

foreach (var tx in txs)
{
    var date = DateTimeOffset.FromUnixTimeMilliseconds(tx.BlockTimestamp).LocalDateTime;

    var v = tx.RawData.Contract[0].Parameter.Value;
    Console.WriteLine($"TX: {tx.TxId}");
    Console.WriteLine($"Date : {date:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine($"From: {v.From}");
    Console.WriteLine($"To: {v.To}");
    Console.WriteLine($"Amount: {v.Amount / 1_000_000m} TRX");

    var fee = ((tx.Receipt?.NetFee ?? 0) + (tx.Receipt?.EnergyFee ?? 0)) / 1_000_000m;

    Console.WriteLine($"Fee: {fee} TRX");
    Console.WriteLine($"Status: {tx.Result[0].Status}");
    Console.WriteLine("--------------");
}

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



