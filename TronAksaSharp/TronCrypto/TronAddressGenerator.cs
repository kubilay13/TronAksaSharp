using Org.BouncyCastle.Crypto.Digests;
using SimpleBase;
using System.Security.Cryptography;

namespace TronAksaSharp.TronCrypto
{
    public class TronAddressGenerator
    {
        public static string PublicKeyToAddress(byte[] publicKey)
        {
            byte[] pubKeyNoPrefix = publicKey.Skip(1).ToArray();

            var digest = new KeccakDigest(256);
            digest.BlockUpdate(pubKeyNoPrefix, 0, pubKeyNoPrefix.Length);
            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);

            byte[] last20 = hash.Skip(12).ToArray(); 
            byte[] tronaddressBytes = new byte[21];
            tronaddressBytes[0] = 0x41;
            Array.Copy(last20, 0, tronaddressBytes, 1, 20);

            byte[] checksum = DoubleSha256(tronaddressBytes).Take(4).ToArray();
            byte[] finalWithChecksum = tronaddressBytes.Concat(checksum).ToArray();

            return Base58.Bitcoin.Encode(finalWithChecksum);
        }

        private static byte[] DoubleSha256(byte[] data)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(sha.ComputeHash(data));
        }
    }
}
