

<img width="928" height="1120" alt="TronAksaSharp" src="https://github.com/user-attachments/assets/db55cac1-fa1c-478f-8fa7-84abc826ab58" />




# TronAksaSharp

**TronAksaSharp**: C# ile yazılmış, Tron ağı için yerli ve açık kaynaklı bir cüzdan & adres üretme  kütüphanesidir.  
Bu kütüphane ile kolayca **private key → public key → Tron adresi** dönüşümü yapabilirsiniz.

## Özellikler

- Rastgele **Private Key** üretimi
- Private Key’den **Public Key** türetme
- Public Key’den **Tron Adresi** hesaplama
- Base58Check formatında okunabilir adres üretimi
- Hafif, sade ve anlaşılır kod yapısı

---

## Yakında Gelecek Özellikler :
- **TRX Transferi** - Tron ağında TRX gönderme/alma
- **TRC20 Token Desteği** - USDT, BTT gibi token işlemleri
- **Akıllı Kontrat Etkileşimi** - Tron smart contract'ları ile çalışma
- **Bakiye Sorgulama** - Adres bakiyelerini öğrenme
- **İşlem Geçmişi** - Transfer geçmişini görüntüleme

---
  
## Kurulum : 

1. Projenize `TronAksaSharp` kütüphanesini ekleyin.
---
2. Gerekli bağımlılıkları yükleyin:

   - [BouncyCastle](https://www.nuget.org/packages/BouncyCastle/)
   - [SimpleBase](https://www.nuget.org/packages/SimpleBase/)
   - [SHA3.Net](https://www.nuget.org/packages/SHA3.Net/)

   Bu paketleri NuGet Paket Yöneticisi ile yükleyebilirsiniz:

   ```bash
   Install-Package BouncyCastle
   Install-Package SimpleBase
   Install-Package SHA3.Net
---
#### Yeni Cüzdan Oluşturma : 
```bash
using TronAksaSharp.TronCrypto;

// Yeni bir cüzdan oluştur
byte[] privateKey = TronKeyGenerator.GeneratePrivateKey();
byte[] publicKey = TronKeyGenerator.PrivateKeyToPublicKey(privateKey);
string address = TronAddressGenerator.PublicKeyToAddress(publicKey);

Console.WriteLine($"Private Key: {BitConverter.ToString(privateKey).Replace("-", "").ToLower()}");
Console.WriteLine($"Public Key: {BitConverter.ToString(publicKey).Replace("-", "").ToLower()}");
Console.WriteLine($"Address: {address}");
```
---
#### Örnek Çıktı : 
```bash
Private Key: 1187c6a8f9f8e6a5f4e3d2c1b0a9f8e7d6c5b4a3f2e1d0c9b8a7f6e5d4c3b2a1
Public Key: 04a7b9c8d6e5f4a3b2c1d0e9f8a7b6c5d4e3f2a1b0c9d8e7f6a5b4c3d2e1f0a9b8c7d6e5f4a3b2c1d0e9f8a7b6c5d4e3f2a1b0c9d8e7f6a5b4c3d2e1f0a9b8
Address: TYqNYnTGsaa9CZaB9FAqRj5WeRxFzK2q6k
```
---

## MIT License : 
```bash
Copyright (c) 2025 Kubilay Efe Akdoğan

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
