using TronAksaSharp.Enums;
using TronAksaSharp.Services;
using TronAksaSharp.Wallet;

static void Step(string title)
{
    Console.WriteLine();
    Console.WriteLine($"════════════ {title} ════════════");
}



//-----------------------------------------------------------------------------
// TRONGRİD HESAP BİLGİLERİ ÇEKME :

Step("ÖRNEK ADRES BİLGİLERİNİ TRONGRİD İLE SORGULAMA");
var service = new TronClient("", TronNetwork.NileTestNet); // Buraya kendi TronGrid API anahtarınızı yazmalısınız
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
//TRONGRİD TRX İŞLEM DETAYLARI ÇEKME :

Step("TRONGRİD İLE TRX İŞLEM DETAYLARI ÇEKME");

var txservice = new TronClient(
    apiKey: "", // Buraya kendi TronGrid API anahtarınızı yazmalısınız
    tronNetwork: TronNetwork.NileTestNet // Buraya sorgulamak istediğiniz TRON ağı (MainNet, NileTestNet, ShastaTestNet)
);

var txs = await txservice.GetTronGridTRXTransactionsDetailsAsync(
    address: "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", // Buraya sorgulamak istediğiniz TRON adresini yazabilirsiniz
    limit: 1 // Çekmek istediğiniz işlem sayısı (max 200) TRONGRİD SINIRI 
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
    Console.WriteLine($"Fee: {tx.FeeTRX} TRX");
    Console.WriteLine($"Status: {tx.Result[0].Status}");
    Console.WriteLine("--------------");
}


//-----------------------------------------------------------------------------
//TRONGRİD TRC-20 İŞLEM DETAYLARI ÇEKME :
Step("TRONGRİD İLE TRC-20 İŞLEM DETAYLARI ÇEKME");
var txstrc20 = await txservice.GetTronGridTRC20TransactionsDetailsAsync(
    address: "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", // Buraya sorgulamak istediğiniz TRON adresini yazabilirsiniz
    limit: 1 // Çekmek istediğiniz işlem sayısı (max 200) TRONGRİD SINIRI 
);
foreach (var tx in txstrc20)
{
    Console.WriteLine($"TXID     : {tx.TransactionId}");
    Console.WriteLine($"From     : {tx.From}");
    Console.WriteLine($"To       : {tx.To}");
    Console.WriteLine($"Amount   : {tx.Amount} {tx.TokenInfo.Symbol}");
    Console.WriteLine($"Fee      : {tx.Fee} TRX");
    Console.WriteLine($"Status   : {tx.Status}");
    Console.WriteLine($"Date     : {tx.Timestamp:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine("------------------------------");
}



//-----------------------------------------------------------------------------
//TRONGRİD FEE HESAPLAMA : 
try
{
    var feeParams = await txservice.GetFeeParametersAsync();

    Console.WriteLine("=== FEE PARAMETRELERİ ===");
    Console.WriteLine($"Transaction Fee: {feeParams.TransactionFee} TRX");
    Console.WriteLine($"Energy Fee: {feeParams.EnergyFee} TRX");
    Console.WriteLine("==========================");
}
catch (Exception ex)
{
    Console.WriteLine($"Hata: {ex.Message}");
}