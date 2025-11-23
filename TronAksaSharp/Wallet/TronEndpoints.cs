using TronAksaSharp.Enums;

namespace TronAksaSharp.Wallet
{
    public class TronEndpoints
    {
        public static string GetBaseUrl(TronNetwork tronNetwork)=> tronNetwork switch
        {
            TronNetwork.MainNet => "https://api.trongrid.io",
            TronNetwork.ShastaTestNet => "https://api.shasta.trongrid.io",
            TronNetwork.NileTestNet => "https://nile.trongrid.io",
            _ => throw new ArgumentOutOfRangeException(nameof(tronNetwork), tronNetwork, null)
        };
        
    }
}
