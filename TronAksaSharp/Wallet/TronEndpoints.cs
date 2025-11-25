using TronAksaSharp.Enums;

namespace TronAksaSharp.Wallet
{
    public class TronEndpoints
    {
        /// <summary>
        /// TRON Ağlarının Url adresleri.
        /// </summary>
        public static string GetBaseUrl(TronNetwork tronNetwork)=> tronNetwork switch
        {
            TronNetwork.MainNet => "https://api.trongrid.io", //MainNet
            TronNetwork.ShastaTestNet => "https://api.shasta.trongrid.io", //ShastaTestNet
            TronNetwork.NileTestNet => "https://nile.trongrid.io", //NileTestNet
            _ => throw new ArgumentOutOfRangeException(nameof(tronNetwork), tronNetwork, null)
        };
    }
}
