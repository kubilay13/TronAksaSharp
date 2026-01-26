using System.Numerics;
using System.Text;

namespace TronAksaSharp.Endcoding
{
    public static class Base58
    {
        private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string Encode(byte[] data)
        {
            BigInteger intData = new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());
            StringBuilder result = new StringBuilder();
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result.Insert(0, Alphabet[remainder]);
            }
            foreach (byte b in data)
            {
                if (b == 0) result.Insert(0, '1');
                else break;
            }
            return result.ToString();
        }

        public static byte[] Decode(string s)
        {
            BigInteger intData = BigInteger.Zero;
            foreach (char c in s)
            {
                int digit = Alphabet.IndexOf(c);
                if (digit < 0) throw new FormatException($"Invalid Base58 character `{c}`");
                intData = intData * 58 + digit;
            }

            int leadingZeros = s.TakeWhile(c => c == '1').Count();
            var bytesWithoutSign = intData.ToByteArray().Reverse().SkipWhile(b => b == 0).ToArray();
            return Enumerable.Repeat((byte)0, leadingZeros).Concat(bytesWithoutSign).ToArray();
        }
    }
}
