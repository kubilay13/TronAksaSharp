using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System.Security.Cryptography;
using TronAksaSharp.Address;

namespace TronAksaSharp.TronCrypto
{
    public class TronTransactionSigner
    {
        public static string Sign(string rawDataHex, byte[] privateKey, string expectedAddress)
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

            var r = sig[0];
            var s = sig[1];

            if (s.CompareTo(domain.N.ShiftRight(1)) > 0)
                s = domain.N.Subtract(s);

            int recId = -1;

            for (int i = 0; i < 4; i++)
            {
                var q = RecoverFromSignature(i, r, s, hash, curve);
                if (q == null) continue;

                byte[] pub = q.GetEncoded(false).Skip(1).ToArray();
                byte[] addr = Keccak256(pub).Skip(12).ToArray();
                string recovered = AddressConverter.HexToBase58("41" + Convert.ToHexString(addr));

                if (recovered == expectedAddress)
                {
                    recId = i;
                    break;
                }
            }

            if (recId == -1)
                throw new Exception("Recovery ID bulunamadı");

            byte[] sigBytes = new byte[65];
            r.ToByteArrayUnsigned().CopyTo(sigBytes, 32 - r.ToByteArrayUnsigned().Length);
            s.ToByteArrayUnsigned().CopyTo(sigBytes, 64 - s.ToByteArrayUnsigned().Length);
            sigBytes[64] = (byte)recId;

            return Convert.ToHexString(sigBytes).ToLower();
        }

        private static Org.BouncyCastle.Math.EC.ECPoint? RecoverFromSignature(int recId, BigInteger r, BigInteger s, byte[] hash, X9ECParameters curve)
        {
            BigInteger n = curve.N;
            BigInteger i = BigInteger.ValueOf(recId / 2);
            BigInteger x = r.Add(i.Multiply(n));

            var prime = ((FpCurve)curve.Curve).Q;
            if (x.CompareTo(prime) >= 0) return null;

            var xBytes = PadLeft32(x.ToByteArrayUnsigned());

            Org.BouncyCastle.Math.EC.ECPoint R = curve.Curve.DecodePoint(new byte[] { (byte)(recId % 2 == 1 ? 0x03 : 0x02) }.Concat(xBytes).ToArray());

            if (!R.Multiply(n).IsInfinity) return null;

            BigInteger e = new BigInteger(1, hash);
            BigInteger rInv = r.ModInverse(n);
            BigInteger srInv = s.Multiply(rInv).Mod(n);
            BigInteger eNeg = n.Subtract(e).Multiply(rInv).Mod(n);

            return curve.G.Multiply(eNeg).Add(R.Multiply(srInv));
        }

        private static byte[] Keccak256(byte[] input)
        {
            var d = new Org.BouncyCastle.Crypto.Digests.KeccakDigest(256);
            d.BlockUpdate(input, 0, input.Length);
            byte[] o = new byte[32];
            d.DoFinal(o, 0);
            return o;
        }

        public static byte[] PadLeft32(byte[] input)
        {
            if (input.Length == 32) return input;

            byte[] result = new byte[32];
            Buffer.BlockCopy(
                input,
                0,
                result,
                32 - input.Length,
                input.Length);

            return result;
        }
    }
}
   
        
