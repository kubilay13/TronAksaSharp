using TronAksaSharp.Models;
using TronAksaSharp.TronCrypto;

namespace TronAksaSharp.Wallet
{
    public class TronWallet
    {
        public static Models.Wallet CreateTronWallet()
        {
            /// <summary>
            /// Tron cüzdanı oluşturur.
            /// </summary>
            var privateKey = KeyGenerator.GeneratePrivateKey(); // Yeni bir özel anahtar oluşturur
            var publicKey = KeyGenerator.PrivateKeyToPublicKey(privateKey); // Özel anahtardan genel anahtar türetir
            var address = AddressGenerator.PublicKeyToAddress(publicKey); // Genel anahtardan Tron adresi oluşturur

            return new Models.Wallet
            {
                PrivateKey = privateKey,
                PublicKey = publicKey,
                Address = address
            };
        }
    }
}
