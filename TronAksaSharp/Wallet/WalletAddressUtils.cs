using TronAksaSharp.Utils;

namespace TronAksaSharp.Wallet
{
    public class WalletAddressUtils
    {
        /// <summary>
        /// TRON cüzdan adresinin uzunluğunu byte cinsinden döner.
        /// </summary>
        public static int WalletAddressLenght(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("Adres boş olamaz");
            }
            byte[] addressBytes = Base58.Decode(address);
            return addressBytes.Length;
        }
    }
}
