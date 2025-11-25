using TronAksaSharp.Enums;
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

var staked = await BalanceService.GetBandwidthStakeAsync(walletAddress, TronNetwork.NileTestNet);
Console.WriteLine("Stake Edilmiş Bandwith TRX: " + staked);

staked = await BalanceService.GetEnergyStakeAsync(walletAddress, TronNetwork.NileTestNet);
Console.WriteLine("Stake Edilmiş Energy TRX: " + staked);




