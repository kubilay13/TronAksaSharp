using TronAksaSharp.Endcoding;

namespace TronAksaSharp.Address
{
    public class AddressConverter
    {
        /// <summary>
        /// Base58Check adresi 21 byte HEX formatına çevirir (owner_address için)
        /// </summary>
        public static string ToHex21(string address)
        {
            byte[] addressBytes = Base58.Decode(address); // 25 byte (21 byte address + 4 byte checksum)
                                                          // İlk 21 byte'ı al (checksum'ı atla)
            byte[] first21 = addressBytes.Take(21).ToArray();
            return BitConverter.ToString(first21).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Base58Check adresi 32 byte parameter formatında HEX'e çevirir (balanceOf için)
        /// </summary>
        public static string ToHex32Parameter(string address)
        {
            byte[] addressBytes = Base58.Decode(address);
            byte[] addr20 = addressBytes.Skip(1).ToArray(); // son 20 byte
            return BitConverter.ToString(addr20).Replace("-", "").PadLeft(64, '0').ToLower();
        }
    }
}
