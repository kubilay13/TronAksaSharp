using System.Security.Cryptography;
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

            // 1 byte version + 20 byte address
            byte[] addr20 = addressBytes.Skip(1).Take(20).ToArray(); 

            return BitConverter.ToString(addr20).Replace("-", "").PadLeft(64, '0').ToLower();
        }

        /// <summary>
        /// 21 byte HEX (41 + 20 byte) adresi Base58Check'e çevirir
        /// (privateKey -> signer address için)
        /// </summary>
        public static string HexToBase58(string hex)
        {
            byte[] data = Convert.FromHexString(hex);

            using var sha256 = SHA256.Create();
            byte[] hash1 = sha256.ComputeHash(data);
            byte[] hash2 = sha256.ComputeHash(hash1);

            byte[] checksum = hash2.Take(4).ToArray();

            byte[] buffer = new byte[data.Length + 4];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
            Buffer.BlockCopy(checksum, 0, buffer, data.Length, 4);

            return Base58.Encode(buffer);
        }
    }
}
