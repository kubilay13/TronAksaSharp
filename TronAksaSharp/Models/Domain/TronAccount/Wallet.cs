namespace TronAksaSharp.Models.Domain.TronAccount
{
    public class Wallet
    {
        public byte[] PrivateKey { get; set; }
        public byte[] PublicKey { get; set; }
        public string Address { get; set; }
        public string PrivateKeyHex => Convert.ToHexString(PrivateKey).ToLower();
        public string PublicKeyHex => Convert.ToHexString(PublicKey).ToLower();

    }
}
