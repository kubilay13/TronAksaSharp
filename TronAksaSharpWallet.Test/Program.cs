using TronAksaSharp.TronCrypto;

// TRON ADDRESS GENERATE :
var privateKey = TronKeyGenerator.GeneratePrivateKey();
Console.WriteLine("Private Key: " + Convert.ToHexString(privateKey).ToLower());


var publicKey = TronKeyGenerator.PrivateKeyToPublicKey(privateKey);
Console.WriteLine("Public Key: " + Convert.ToHexString(publicKey).ToLower());


var address = TronAddressGenerator.PublicKeyToAddress(publicKey);
Console.WriteLine("Tron Address: " + address);