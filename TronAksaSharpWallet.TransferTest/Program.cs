using TronAksaSharp.Enums;
using TronAksaSharp.Wallet;

//-----------------------------------------------------------------------------
// TRX TRANSFER TESTİ (DÜZENLENMİŞ)

string senderAddress = "TMYMQWbWrKnK4QLeLD7QWhcE38t2vH3wto";
string senderPrivateKey = "46599a45b3f178f6406f3b53f4b4f61f0cd9b6d4b2dab318c765d5d2fe78b1b9"; // Gönderen cüzdanın özel anahtarı 
string receiverAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

decimal sendAmount = 1m; // 1 TRX

bool transferResult = await TronClient.SendTRXAsync(
    senderAddress,
    senderPrivateKey,
    receiverAddress,
    sendAmount,
    TronNetwork.NileTestNet
);

Console.WriteLine("TRX Transfer Sonucu : " + transferResult);

