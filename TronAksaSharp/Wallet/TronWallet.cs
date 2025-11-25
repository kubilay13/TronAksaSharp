using TronAksaSharp.Models;
using TronAksaSharp.TronCrypto;

namespace TronAksaSharp.Wallet
{
    public class TronWallet
    {
        public static WalletModel CreateTronWallet()
        {
            /// <summary>
            /// Tron cüzdanı oluşturur.
            /// </summary>
            var privateKey = TronKeyGenerator.GeneratePrivateKey(); // Yeni bir özel anahtar oluşturur
            var publicKey = TronKeyGenerator.PrivateKeyToPublicKey(privateKey); // Özel anahtardan genel anahtar türetir
            var address = TronAddressGenerator.PublicKeyToAddress(publicKey); // Genel anahtardan Tron adresi oluşturur

            return new WalletModel
            {
                PrivateKey = privateKey,
                PublicKey = publicKey,
                Address = address
            };
        }
    }
}
