using System;
using TronAksaSharp.Enums;
using TronAksaSharp.TronCrypto;
using TronAksaSharp.Utils;
using TronAksaSharp.Wallet;


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
// TRX TRANSFER TESTİ :

string senderAddress = "SENDER ADDRESS";
string senderPrivateKey = "SENDER PRİVATE KEY";

byte[] privateKeyBytes = Convert.FromHexString(senderPrivateKey);

string receiverAddress = "RECEİVER ADDRESS";
decimal sendAmount = 1m; // Gönderilecek TRX miktarı ÖR 1 TRX

var tx = await TronTransferService.CreateTRXTransactionAsync(
    senderAddress,
    receiverAddress,
    sendAmount,
    TronNetwork.NileTestNet // Nile TestNet
);

string rawHex = tx.RootElement
    .GetProperty("raw_data_hex")
    .GetString();

string signature = TronTransactionSigner.Sign(rawHex, privateKeyBytes);

bool transferResult = await TronTransferService.BroadcastAsync(
    tx,
    signature,
    TronNetwork.NileTestNet
);

Console.WriteLine("TRX Transfer Sonucu : " + transferResult);




