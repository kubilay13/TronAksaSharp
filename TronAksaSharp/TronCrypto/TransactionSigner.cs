using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using System.Security.Cryptography;

namespace TronAksaSharp.TronCrypto
{
    public class TronTransactionSigner
    {
        public static string Sign(string rawDataHex, byte[] privateKey)
        {
            byte[] rawBytes = Convert.FromHexString(rawDataHex);

            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(rawBytes);

            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N);
            var d = new BigInteger(1, privateKey);
            var privKey = new ECPrivateKeyParameters(d, domain);

            var signer = new ECDsaSigner();
            signer.Init(true, privKey);

            var sig = signer.GenerateSignature(hash);

            byte[] r = sig[0].ToByteArrayUnsigned();
            byte[] s = sig[1].ToByteArrayUnsigned();

            byte[] signature = new byte[65];
            Buffer.BlockCopy(r, 0, signature, 32 - r.Length, r.Length);
            Buffer.BlockCopy(s, 0, signature, 64 - s.Length, s.Length);
            signature[64] = 0x01; // recovery id (tron için sabit)

            return Convert.ToHexString(signature).ToLower();
        }
    }
}
