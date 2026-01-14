

<img width="928" height="1120" alt="TronAksaSharp" src="https://github.com/user-attachments/assets/db55cac1-fa1c-478f-8fa7-84abc826ab58" />




# TronAksaSharp

**TronAksaSharp**: C# ile yazılmış, Tron ağı için yerli ve açık kaynaklı bir cüzdan & adres üretme  kütüphanesidir.  
Bu kütüphane ile kolayca **private key → public key → Tron adresi** dönüşümü yapabilirsiniz.

---
## Özellikler:

- Public Key’den **Tron Adresi** Oluşturma
- Base58Check formatında okunabilir adres üretimi (bağımsız Base58 implementasyonu)
- Adres byte uzunluğu kontrolü
- Cüzdanın TRX ve TRC-20 token kontrolü (MainNet,Nile,Shasta) Dahil
- TRX ve TRC20 Transferi (MainNet,Nile,Shasta) Dahil
- Cüzdanın stake edilen varlıkların kontrolu (Energy,Bandwidth)

---

## Yakında Gelecek Özellikler :
- **Akıllı Kontrat Etkileşimi** - Tron smart contract'ları ile çalışma
- İşlemlerin Detaylarını görme
- Cüzdan Dinleme
  

---
  
## Kurulum : 

1. Projenize `TronAksaSharp` kütüphanesini ekleyin.
   NuGet : https://www.nuget.org/packages/TronAksaSharp
---
2. Gerekli bağımlılıkları yükleyin:

   - [BouncyCastle](https://www.nuget.org/packages/BouncyCastle/)

   Bu paketleri NuGet Paket Yöneticisi ile yükleyebilirsiniz:

   ```bash
   Install-Package BouncyCastle
---
NOT: Base58 işlemleri artık kütüphane içinde, SimpleBase bağımlılığı kaldırıldı.

#### Yeni Cüzdan Oluşturma : 
```bash

string ToHex(byte[] data) => BitConverter.ToString(data).Replace("-", "");
var createWallet = TronClient.CreateTronWallet();

Console.WriteLine($"Cüzdan Adresi :\n {createWallet.Address}");
Console.WriteLine($"Private Key :\n {ToHex(createWallet.PrivateKey)}");
Console.WriteLine($"Public Key  :\n {ToHex(createWallet.PublicKey)}");

```

#### Örnek Çıktı : 
```bash
Private Key (hex): 03FFEB1D127C5BEF8377F32092F3AD4FEEC93D337553CF5DC8120EC9838147DC
Public Key (hex) : 046AF87108AD9870550EF651B18D24A551391D66E73BA44DCB45AE7232955FC19D6F6190B58BD3B5D1D96CD127D423B72D70900A972ACD48BD42C35C30098416E8
Address          : TUqSfg8fT6t4R5zn2Lhe1RtQ6vHmJLeyJt
```
---
## Adres Uzunluk Hesaplama :
```bash

byte[] addrBytes = Base58.Decode($"{createWallet.Address}");
Console.WriteLine("Adres byte uzunluğu = " + addrBytes.Length);

```
---

## TRX BAKİYE-STAKE(ENERGY,BANDWİTH) SORGULAMA :
```bash

string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"; // Örnek TRON adresi

var balances = await TronClient.GetBalancesAsync(walletAddress, TronNetwork.NileTestNet);

Console.WriteLine($"TRX       : {balances.TrxBalance}");
Console.WriteLine($"Bandwidth : {balances.BandwidthStake}");
Console.WriteLine($"Energy    : {balances.EnergyStake}");

```
### Örnek Çıktı : 
```bash
TRX Balance (Nile): 45575,325323
TRX Balance (Shasta): 9957,8
TRX Balance (MainNet): 0
Stake Edilmiş Bandwith TRX: 24938
Stake Edilmiş Energy TRX: 526780
```
## TRC20 TOKEN BAKİYE SORGULAMA :
```bash
string trc20Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT TRC20 Contract Address
int decimals = 6; // USDT TRC20 token ondalık basamak sayısı
decimal tokenBalance = await BalanceService.GetTRC20BalanceAsync(walletAddress, trc20Contract, decimals, TronNetwork.NileTestNet); // Nile TestNet
Console.WriteLine($"TRC20 Token Balance: {tokenBalance}");

```
### Örnek Çıktı : 
```bash
TRC20 Token Balance: 110430
```
## TRX TRANSFER :
```bash

var trx = await TronClient.SendTRXAsync(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    1,
    TronNetwork.NileTestNet
);

Console.WriteLine(trx.Success ? "TRX Transferi Başarılı" : $"TRX HATA → {trx.Error}");

```
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

Console.WriteLine(trc20.Success ? "TRC20 Transferi Başarılı" : $"TRC20 HATA → {trc20.Error}");

```

---

## MIT License : 
```bash
Copyright (c) 2025 Kubilay13

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
