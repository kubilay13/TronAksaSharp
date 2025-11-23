using TronAksaSharp.Models;
using TronAksaSharp.TronCrypto;

namespace TronAksaSharp.Wallet
{
    public class TronWallet
    {
        public static WalletModel CreateTronWallet()
        {
            var privateKey = TronKeyGenerator.GeneratePrivateKey();
            var publicKey = TronKeyGenerator.PrivateKeyToPublicKey(privateKey);
            var address = TronAddressGenerator.PublicKeyToAddress(publicKey);

            return new WalletModel
            {
                PrivateKey = privateKey,
                PublicKey = publicKey,
                Address = address
            };
        }
    }
}
