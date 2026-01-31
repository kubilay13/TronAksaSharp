namespace TronAksaSharp.Models.Domain.TronAccount
{
    public class Wallet
    {
        public byte[] PrivateKey { get; set; }
        public byte[] PublicKey { get; set; }
        public string Address { get; set; }
    }
}
