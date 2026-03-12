

<img width="928" height="1120" alt="TronAksaSharp" src="https://github.com/user-attachments/assets/b1192ba6-23db-45ba-9697-c78ee56b302d" />


# TronAksaSharp

**TronAksaSharp**: C# ile yazılmış, Tron ağı için yerli ve açık kaynaklı bir cüzdan & adres üretme  kütüphanesidir.  
Bu kütüphane ile kolayca **private key → public key → Tron adresi** dönüşümü yapabilirsiniz.

---
## Özellikler:

- Public Key’den **Tron Adresi** Oluşturma
- Base58Check formatında okunabilir adres üretimi (bağımsız Base58 implementasyonu)
- Adres byte uzunluğu kontrolü
- Cüzdanın TRX ve TRC-20 token kontrolü (MainNet,Nile,Shasta) Dahil
- TRX ve TRC20 Transfer İşlemleri (MainNet,Nile,Shasta) Dahil
- Cüzdanın stake edilen varlıkların kontrolu (Energy,Bandwidth)
- İşlemlerin Detaylarını görme
- Cüzdan Otomasyon Transferleri

---

## Yakında Gelecek Özellikler :
- **Akıllı Kontrat Etkileşimi** - Tron smart contract'ları ile çalışma
  
---
  
## Kurulum : 

1. Projenize `TronAksaSharp` kütüphanesini ekleyin.
   NuGet : https://www.nuget.org/packages/TronAksaSharp
---
NOT: Base58 işlemleri artık kütüphane içinde, SimpleBase bağımlılığı kaldırıldı.
---

## YENİ ADRES OLUŞTURMA KODU : 
```bash

string ToHex(byte[] data) => BitConverter.ToString(data).Replace("-", "");
var createWallet = TronClient.CreateTronWallet();

Console.WriteLine($"Cüzdan Adresi :\n {createWallet.Address}");
Console.WriteLine($"Private Key :\n {createWallet.PrivateKeyHex}"); 
Console.WriteLine($"Public Key  :\n {createWallet.PublicKeyHex}");

```

## YENİ ADRES OLUŞTURMA KODU ÇIKTI ÖRNEĞİ : 
```bash
Cüzdan Adresi :
 TLLEyjRhHdg7QJLpDxR9m35DWMfo7Fe2jq
Private Key :
 8fcef3d79e87df12c420f96a6cfae8414c5c35a410d8b12605c6bdad3900373f
Public Key  :
 04cc8ddf9ac9266c1e065267be2e54d82a689da718b6fdbc90901b7c4c031e188b37ed065937ef730200f20cc486d272636c0ed9dc929b066c00e47fac66f47c3b
```
---
## ADRES UZUNLUK HESAPLAMA :
```bash

byte[] addrBytes = Base58.Decode($"{createWallet.Address}");
Console.WriteLine("Adres byte uzunluğu = " + addrBytes.Length);

```
## ADRES UZUNLUK HESAPLAMA ÇIKTI ÖRNEĞİ :
```bash

Adres byte uzunluğu = 25

```
---

## TRX BAKİYE-STAKE(ENERGY,BANDWİTH) SORGULAMA :
```bash

Step("BAKİYE & STAKE MANUEL RPC ÇAĞRISI");
string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"; // Örnek TRON adresi

var balances = await TronClient.GetBalancesAsync(walletAddress, TronNetwork.NileTestNet);

Console.WriteLine($"TRX       : {balances.TrxBalance}");
Console.WriteLine($"Bandwidth : {balances.BandwidthStake}");
Console.WriteLine($"Energy    : {balances.EnergyStake}");


```
### Örnek Çıktı : 
```bash
TRX       : 761295,371631
Bandwidth : 24938
Energy    : 100000
```
---
## TRC20 TOKEN BAKİYE SORGULAMA :
```bash
Step("TRC20 BAKİYE MANUEL RPC ÇAĞRISI");
string trc20Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT TRC20 Contract Address
decimal usdtBalance = await TronClient.GetTRC20BalanceAsync(
    walletAddress,
    trc20Contract,
    6, // USDT decimals
    TronNetwork.NileTestNet
);
```
### Örnek Çıktı : 
```bash
TRC20 Token Balance: 110430
```
---
## TRONGRİD APİ İLE ADRES BİLGİLERİNİ SORGULAMA:
```bash
tep("ÖRNEK ADRES BİLGİLERİNİ TRONGRİD İLE SORGULAMA");
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

```

##  TRONGRİD APİ İLE ADDRESS BİLGİLERİNİ SORGULAMA ÇIKTI ÖRNEĞİ :
```bash
════════════ ÖRNEK ADRES BİLGİLERİNİ TRONGRİD İLE SORGULAMA ════════════
====== TRON ACCOUNT ======
Adres: 4131c1fe443e54d007fd1c8c5e7ae7c2356b374616
Bakiye (TRX): 761295,371631
Oluşturulma: 1.07.2024 08:33:24 +00:00

====== FROZEN ======
: 24938 TRX
ENERGY: 100000 TRX
TRON_POWER: 0 TRX

====== ENERGY / RESOURCE ======
Energy Optimized: True
Energy Window Size: 28800000
Son Energy Harcama: 1767721134000

====== PERMISSIONS ======
OWNER:
Threshold: 1
 - TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N (Weight: 1)
ACTIVE:
Permission: active
Threshold: 1
Operations: 7fff1fc0033efb0f000000000000000000000000000000000000000000000000

====== VOTES ======
SR: TFMehqCGCei9RrJLdM5eRFuwHY4CrYy6Xt | Oy: 124938

====== TRC20 TOKENS ======
Token: TNDSHKGBmgRx9mDYA9CnxPx55nu672yQw2
Balance (raw): 10000000000
Token: TF17BgPaZYbz8oxbjhriubPDsA7ArKoLX3
Balance (raw): 1000000000000000000000
Token: TFT7sNiNDGZcqL7z7dwXUPpxrx1Ewk8iGL
Balance (raw): 23900100000000000000000
Token: TVSvjZdyDSNocHm7dP3jvCmMNsCnMTPa5W
Balance (raw): 21099016200000000000000000
Token: TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf
Balance (raw): 128692000000
Token: TXLAQ63Xg1NAzckPwKHvzw7CSEmLMEqcdj
Balance (raw): 258900000000
Token: TNuoKL1ni8aoshfFL1ASca1Gou9RXwAzfn
Balance (raw): 179931469700000016307453952
Token: TEMVynQpntMqkPxP6wXTW2K7e4sM3cRmWz
Balance (raw): 64014500000
====== BİTTİ ======
```
---
## TRONGRİD APİ İLE TRX İŞLEM DETAYLARI ÇEKME :
```bash
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

```
## TRONGRİD APİ İLE TRX İŞLEM DETAYLARI ÇEKME ÇIKTI ÖRNEĞİ :
```bash
TX: d30f27b6e43b621ca0d4833dc5758add0eb2e9f843eed6a747d25d68ebb6ca95
Date : 2026-02-16 05:17:06
From: 417eee203e9e24c250a7e657cd8cbe1376fe250763
To: 4131c1fe443e54d007fd1c8c5e7ae7c2356b374616
Amount: 10 TRX
Fee: 0 TRX
Status: SUCCESS
--------------
TX: 006fc32ad4c8ace60b6a8414c69765aa374ab356413fa4faf707fdce19a3e695
Date : 2026-02-14 22:32:42
From: 41d093f24888ab06073a4bdffbb8107db1ea9dc0a0
To: 4131c1fe443e54d007fd1c8c5e7ae7c2356b374616
Amount: 5000 TRX
Fee: 0 TRX
Status: SUCCESS
--------------
TX: 5a94f007b5671a8e5d901fffb27ca3bf98716aa6917560b3a6a4726b097b4388
Date : 2026-02-13 00:11:09
From: 4109329fc0a273342757ee4adaa2f9a13371118041
To: 4131c1fe443e54d007fd1c8c5e7ae7c2356b374616
Amount: 1,22364 TRX
Fee: 0 TRX
Status: SUCCESS
--------------
```
---
## TRONGRİD APİ İLE TRC-20 İŞLEM DETAYLARI ÇEKME :
```bash
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
```
---
## TRONGRİD APİ İLE TRC-20 İŞLEM DETAYLARI ÇEKME ÇIKTI ÖRNEĞİ :
```bash
════════════ TRONGRİD İLE TRC-20 İŞLEM DETAYLARI ÇEKME ════════════
TXID     : b517eb4427380efa8ef3d04741dd049c216849654ccda2e78db2121abfab5f1d
From     : TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N
To       : TGir1bGvZYoWoER3gkHHCSXAFKftQVQpyC
Amount   : 3 USDC
Fee      : 0 TRX
Status   : SUCCESS
Date     : 2026-03-07 00:58:36
------------------------------
```
---
## TRX TRANSFER :
```bash

var trx = await TronClient.SendTRXAsync(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    1,
    TronNetwork.NileTestNet
);

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

```
## TRX TRANSFER ÇIKTISI ÖRNEĞİ :
```bash
════════════ TRX TRANSFER ════════════
FROM    = TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto
SIGNER  = TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto
PERM_ID = 0
----- TRX TRANSFER INFO RPC MANUEL ÇAĞRISI -----
Block Number : 64915530
Txn Hash : d30f27b6e43b621ca0d4833dc5758add0eb2e9f843eed6a747d25d68ebb6ca95
Timestamp    : 16.02.2026 02:17:03
From        : TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto
To          : TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N
Amount      : 10 TRX
Result      : Successful
Fee (SUN)   : 0
Energy Used : 0
Net Fee     : 0
-------------------
TRX Transferi Başarılı Şekilde Gönderildi.
```
---
## TRC20 TRANSFER :
```bash

string usdtContract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT (Nile)
int decimals = 6; // USDT için ondalık basamak sayısı

var trc20 = await TronClient.SendTRC20Async(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    usdtContract,
    1,
    6,
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

```
## TRC20 TRANSFER ÇIKTISI ÖRNEĞİ:
```bash
════════════ TRC20 TRANSFER ════════════
FROM    = TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto
SIGNER  = TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto
PERM_ID = 0
----- TRC20 TRANSFER INFO RPC MANUEL ÇAĞRISI -----
Block Number : 64915532
Txn Hash     : f3b45403730335c0ae2a5daf240328fabd455f78922ac9853985876399d65509
Timestamp    : 16.02.2026 02:17:10
From         : TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto
To           : TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N
Amount       : 10 TRC20:TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf
Result       : Successful
Fee (SUN)    : 345000
Energy Used  : 14650
Net Fee      : 345000
--------------------------
TRC20 Transferi Başarılı Şekilde Gönderildi.
```
---
## TRON OTOMATİK TRANSFER :
```bash
var client = new TronClient("API_KEY", TronNetwork.NileTestNet);

var watchAddress = "TYkrpvjHfVcuqRei7pDbSiU67CriYHNrvm"; 
var watchPrivateKey = "b89e495325fe92898d8c6a7f7b18bac99e0c69af447a53bf37092eb0f98d29c7";
var forwardAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";
var network = TronNetwork.NileTestNet;
var minReserve = 1;
var checkIntervalSeconds = 10;


await client.StartForwardingAsync(watchAddress,watchPrivateKey,forwardAddress,minReserve,checkIntervalSeconds);
```


## MIT License : 
```bash
Copyright (c) 2026 Kubilay13

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

```
