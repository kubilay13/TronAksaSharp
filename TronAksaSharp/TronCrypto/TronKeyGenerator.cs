using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace TronAksaSharp.TronCrypto
{
    public class TronKeyGenerator
    {
        public static byte[] GeneratePrivateKey()
        {
            var ecParams = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(ecParams.Curve, ecParams.G, ecParams.N, ecParams.H);

            var gen = new ECKeyPairGenerator();
            var keyParams = new ECKeyGenerationParameters(domain, new Org.BouncyCastle.Security.SecureRandom());
            gen.Init(new ECKeyGenerationParameters(domain, new Org.BouncyCastle.Security.SecureRandom()));
            var keyPair = gen.GenerateKeyPair();
            return Fix32(((ECPrivateKeyParameters)keyPair.Private).D.ToByteArrayUnsigned());
        }

        public static byte[] PrivateKeyToPublicKey(byte[] privateKey)
        { 
            var ecParams = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(ecParams.Curve, ecParams.G, ecParams.N, ecParams.H);
            var d = new Org.BouncyCastle.Math.BigInteger(1, privateKey);
            var q = domain.G.Multiply(d).Normalize();

            return q.GetEncoded(false);
        }
        private static byte[] Fix32(byte[] key)
        {
            if (key.Length == 32) return key;
            var fixedKey = new byte[32];
            Buffer.BlockCopy(key, 0, fixedKey, 32 - key.Length, key.Length);
            return fixedKey;
        }
    }
}
