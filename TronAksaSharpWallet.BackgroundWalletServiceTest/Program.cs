using TronAksaSharp.Enums;
using TronAksaSharp.Wallet;

//-----------------------------------------------------------------------------
// TRON OTOMASYON TRANSFER :

// 1. Ağ ayarı (TestNet kullanıyoruz)
var network = TronNetwork.NileTestNet;

// 2. İzlenecek cüzdan (BU ADRESE PARA GELECEK)
var watchAddress = "TYkrpvjHfVcuqRei7pDbSiU67CriYHNrvm";

// 3. İzlenen cüzdanın PRIVATE KEY'i (PARAYI ÇEKMEK İÇİN LAZIM!)
var watchPrivateKey = "b89e495325fe92898d8c6a7f7b18bac99e0c69af447a53bf37092eb0f98d29c7";

// 4. Paranın gönderileceği HEDEF CÜZDAN
var forwardAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

// 5. Cüzdanda bırakılacak minimum TRX (gas ücreti için)
var minReserve = 1;

// 6. Kaç saniyede bir kontrol edilecek
var checkIntervalSeconds = 10;

// 7. Cüzdandan Çekilecek maksimum TRX miktarı (sınırlama)
var maxReserve = 10;

// 8. BAŞLAT!
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🚀 Tron otomasyon başlatılıyor...");
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] İzlenecek: {watchAddress}");
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Hedef: {forwardAddress}");
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Kontrol aralığı: {checkIntervalSeconds} saniye");
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Min reserve: {minReserve} TRX");
Console.WriteLine("------------------------------------------------");

// ✅ DÜZELTİLMİŞ ÇAĞRI - network parametresi eklendi!
await TronClient.StartForwardingAsync(
    watchAddress,
    watchPrivateKey,
    forwardAddress,
    minReserve,
    maxReserve,
    checkIntervalSeconds,
    network  // ✅ network burada veriliyor
);

// BU SATIR HİÇ ÇALIŞMAZ! (StartForwardingAsync sonsuz döngü)
Console.WriteLine("Bu yazıyı göremezsin!");