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
            gen.Init(keyParams);

            var keyPair = gen.GenerateKeyPair();

            var privateKey = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArrayUnsigned();

            return privateKey;
        }

        public static byte[] PrivateKeyToPublicKey(byte[] privateKey)
        { 
            var ecParams = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(ecParams.Curve, ecParams.G, ecParams.N, ecParams.H);
            var d = new Org.BouncyCastle.Math.BigInteger(1, privateKey);
            var q = domain.G.Multiply(d).Normalize();

            return q.GetEncoded(false);
        }
    }
}
