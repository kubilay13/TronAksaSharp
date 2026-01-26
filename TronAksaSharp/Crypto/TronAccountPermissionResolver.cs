using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using System.Text.Json;
using TronAksaSharp.Address;
using Org.BouncyCastle.Math;

namespace TronAksaSharp.Crypto
{
    public static class TronAccountPermissionResolver
    {
        public static string AddressFromPrivateKey(byte[] privateKey)
        {
            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N);

            var d = new BigInteger(1, privateKey);
            var q = domain.G.Multiply(d).Normalize();

            byte[] pubKey = q.GetEncoded(false).Skip(1).ToArray();

            var sha3 = new Org.BouncyCastle.Crypto.Digests.KeccakDigest(256);
            sha3.BlockUpdate(pubKey, 0, pubKey.Length);
            byte[] hash = new byte[32];
            sha3.DoFinal(hash, 0);

            byte[] addr = hash.Skip(12).ToArray();
            return AddressConverter.HexToBase58(
                "41" + Convert.ToHexString(addr));
        }

        public static int ResolvePermissionId(JsonDocument accountDoc, string signerAddress)
        {
            var root = accountDoc.RootElement;

            if (root.TryGetProperty("active_permission", out var activePerms))
            {
                foreach (var perm in activePerms.EnumerateArray())
                {
                    foreach (var key in perm.GetProperty("keys").EnumerateArray())
                    {
                        if (key.GetProperty("address").GetString() == signerAddress)
                        {
                            return perm.GetProperty("id").GetInt32();
                        }
                    }
                }
            }

            return 0;
        }
    }
}
