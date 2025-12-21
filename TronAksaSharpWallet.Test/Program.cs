using TronAksaSharp.Endcoding;
using TronAksaSharp.Enums;
using TronAksaSharp.Wallet;
using TronAksaSharp.Wallet.Services;

//-----------------------------------------------------------------------------


// TRON ADDRESS GENERATE :
string ToHex(byte[] data) => BitConverter.ToString(data).Replace("-", "");
var createWallet = TronWallet.CreateTronWallet();
Console.WriteLine($"Cüzdan Adresi :\n {createWallet.Address}");
Console.WriteLine($"Private Key :\n {ToHex(createWallet.PrivateKey)}");
Console.WriteLine($"Public Key  :\n {ToHex(createWallet.PublicKey)}");


//-----------------------------------------------------------------------------


// ADDRESS BYTE UZUNLUĞU SORGULAMA : 
byte[] addrBytes = Base58.Decode($"{createWallet.Address}");
Console.WriteLine("Adres byte uzunluğu = " + addrBytes.Length);


//-----------------------------------------------------------------------------


// TRX BAKİYE SORGULAMA :
string walletAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N"; // Örnek TRON adresi

decimal trxBalance = await BalanceService.GetTRXBalanceAsync(walletAddress, TronNetwork.NileTestNet); // Nile TestNet
Console.WriteLine($"TRX Balance (Nile): {trxBalance}");

trxBalance = await BalanceService.GetTRXBalanceAsync(walletAddress, TronNetwork.ShastaTestNet); // Shasta TestNet
Console.WriteLine($"TRX Balance (Shasta): {trxBalance}");

trxBalance = await BalanceService.GetTRXBalanceAsync(walletAddress, TronNetwork.MainNet); // MainNet
Console.WriteLine($"TRX Balance (MainNet): {trxBalance}");

//-----------------------------------------------------------------------------


// TRON STAKE BAKİYE SORGULAMA :
var staked = await BalanceService.GetBandwidthStakeAsync(walletAddress, TronNetwork.NileTestNet);
Console.WriteLine("Stake Edilmiş Bandwith TRX: " + staked); // Bandwith için stake edilmiş TRX miktarı

staked = await BalanceService.GetEnergyStakeAsync(walletAddress, TronNetwork.NileTestNet);
Console.WriteLine("Stake Edilmiş Energy TRX: " + staked); // Energy için stake edilmiş TRX miktarı


//-----------------------------------------------------------------------------

// TRC20 TOKEN BAKİYE SORGULAMA :
string trc20Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"; // USDT TRC20 Contract Address
int decimals = 6; // USDT TRC20 token ondalık basamak sayısı
decimal tokenBalance = await BalanceService.GetTRC20BalanceAsync(walletAddress, trc20Contract, decimals, TronNetwork.NileTestNet); // Nile TestNet
Console.WriteLine($"TRC20 Token Balance: {tokenBalance}");

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




