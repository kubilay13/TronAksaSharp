

<img width="928" height="1120" alt="TronAksaSharp" src="https://github.com/user-attachments/assets/db55cac1-fa1c-478f-8fa7-84abc826ab58" />




# TronAksaSharp

**TronAksaSharp**: C# ile yazılmış, Tron ağı için yerli ve açık kaynaklı bir cüzdan & adres üretme  kütüphanesidir.  
Bu kütüphane ile kolayca **private key → public key → Tron adresi** dönüşümü yapabilirsiniz.

---
## Özellikler

- Rastgele **Private Key** üretimi
- Private Key’den **Public Key** türetme
- Public Key’den **Tron Adresi** hesaplama
- Base58Check formatında okunabilir adres üretimi (bağımsız Base58 implementasyonu)
- Adres byte uzunluğu kontrolü
- Cüzdanın TRX ve TRC-20 token kontrolü (MainNet,Nile,Shasta) Dahil
- Cüzdanın stake edilen varlıkların kontrolu (Energy,Bandwidth)

---

## Yakında Gelecek Özellikler :
- **TRX Transferi** - Tron ağında TRX gönderme/alma
- **TRC20 Token Desteği** - USDT, BTT gibi token işlemleri
- **Akıllı Kontrat Etkileşimi** - Tron smart contract'ları ile çalışma
- **İşlem Geçmişi** - Transfer geçmişini görüntüleme

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

using TronAksaSharp.Wallet;
using TronAksaSharp.Wallet; // TronWallet ve AddressUtils
using System;

var wallet = TronWallet.CreateTronWallet();

// Byte dizilerini hex string olarak gösterme
string ToHex(byte[] data) => BitConverter.ToString(data).Replace("-", "").ToLower();

Console.WriteLine($"Private Key (hex): {ToHex(wallet.PrivateKey)}");
Console.WriteLine($"Public Key (hex) : {ToHex(wallet.PublicKey)}");
Console.WriteLine($"Address          : {wallet.Address}");

// Adresin byte uzunluğunu kontrol etme
int addrLength = AddressUtils.GetAddressByteLength(wallet.Address);
Console.WriteLine($"Address byte uzunluğu = {addrLength}");

```

#### Örnek Çıktı : 
```bash
Private Key (hex): 03FFEB1D127C5BEF8377F32092F3AD4FEEC93D337553CF5DC8120EC9838147DC
Public Key (hex) : 046AF87108AD9870550EF651B18D24A551391D66E73BA44DCB45AE7232955FC19D6F6190B58BD3B5D1D96CD127D423B72D70900A972ACD48BD42C35C30098416E8
Address          : TUqSfg8fT6t4R5zn2Lhe1RtQ6vHmJLeyJt
Address byte uzunluğu = 25
```
---
## Hex ve Adres Hesaplama Açıklaması :
```bash
1. Private Key → Public Key
- secp256k1 eliptik eğri algoritması kullanılır.
- Public key uncompressed formatta (65 byte) elde edilir.

2. Public Key → Tron Adresi
- Public key’in ilk byte’ı (0x04) çıkarılır.
- Kalan 64 byte, Keccak-256 hash işleminden geçirilir.
- Hash’in son 20 byte’ı alınır ve başına 0x41 eklenir (TRON adres prefix).
- byte checksum hesaplanır (Double SHA-256).
- Sonuç Base58Check formatına çevrilir → okunabilir Tron adresi elde edilir.

3. Adres Byte Uzunluğu
- Tron Base58Check adresi: 21 byte veri + 4 byte checksum = 25 byte uzunluğundadır.
- AddressUtils.GetAddressByteLength metodu ile bu uzunluğu kolayca doğrulayabilirsiniz.

```
---

## TRX BAKİYE-STAKE(ENERGY,BANDWİTH) SORGULAMA :
```bash
using TronAksaSharp.Wallet;

// TRX BAKİYE SORGULAMA :
string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"; // Örnek TRON adresi

decimal trxBalance = await BalanceService.GetTRXBalanceAsync(walletAddress, TronNetwork.NileTestNet); // Nile TestNet
Console.WriteLine($"TRX Balance (Nile): {trxBalance}");

trxBalance = await BalanceService.GetTRXBalanceAsync(walletAddress, TronNetwork.ShastaTestNet); // Shasta TestNet
Console.WriteLine($"TRX Balance (Shasta): {trxBalance}");

trxBalance = await BalanceService.GetTRXBalanceAsync(walletAddress, TronNetwork.MainNet); // MainNet
Console.WriteLine($"TRX Balance (MainNet): {trxBalance}");

// TRON STAKE BAKİYE SORGULAMA :
var staked = await BalanceService.GetBandwidthStakeAsync(walletAddress, TronNetwork.NileTestNet); // Bandwith TRX Bakiye Sorgulama
Console.WriteLine("Stake Edilmiş Bandwith TRX: " + staked);

staked = await BalanceService.GetEnergyStakeAsync(walletAddress, TronNetwork.NileTestNet); // Energy Trx Bakiye Sorgulama
Console.WriteLine("Stake Edilmiş Energy TRX: " + staked);

// TRC20 TOKEN BAKİYE SORGULAMA :
string trc20Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT TRC20 Contract Address
int decimals = 6; // USDT TRC20 token ondalık basamak sayısı
decimal tokenBalance = await BalanceService.GetTRC20BalanceAsync(walletAddress, trc20Contract, decimals, TronNetwork.NileTestNet); // Nile TestNet
Console.WriteLine($"TRC20 Token Balance: {tokenBalance}");

```

### Örnek Çıktı : 
```bash
TRX Balance (Nile): 45575,325323
TRX Balance (Shasta): 9957,8
TRX Balance (MainNet): 0
Stake Edilmiş Bandwith TRX: 24938
Stake Edilmiş Energy TRX: 526780
TRC20 Token Balance: 110430
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
