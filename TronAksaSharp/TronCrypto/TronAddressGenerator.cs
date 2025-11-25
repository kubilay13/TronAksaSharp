using Org.BouncyCastle.Crypto.Digests;
using TronAksaSharp.Utils;
using System.Security.Cryptography;

namespace TronAksaSharp.TronCrypto
{
    public class TronAddressGenerator
    {
        /// <summary>
        /// Tron adresini verilen açık anahtardan oluşturur.
        /// </summary>
        public static string PublicKeyToAddress(byte[] publicKey)
        {
            if(publicKey.Length !=65 || publicKey[0] !=0x004)
            {
                throw new ArgumentException("Uncompressed public key bekleniyor.");
            }

            byte[] pubKeyNoPrefix = publicKey[1..];

            var digest = new KeccakDigest(256);
            digest.BlockUpdate(pubKeyNoPrefix, 0, pubKeyNoPrefix.Length);
            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);

            byte[] last20 = hash[12..];
            byte[] tronaddressBytes = new byte[21];
            tronaddressBytes[0] = 0x41;
            Array.Copy(last20, 0, tronaddressBytes, 1, 20);

            byte[] checksum = DoubleSha256(tronaddressBytes)[..4];
            byte[] finalWithChecksum = tronaddressBytes.Concat(checksum).ToArray();

            return Base58.Encode(finalWithChecksum);
        }

        private static byte[] DoubleSha256(byte[] data)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(sha.ComputeHash(data));
        }
    }
}
