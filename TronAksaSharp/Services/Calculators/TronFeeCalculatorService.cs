using TronAksaSharp.Enums;
using TronAksaSharp.Wallet;

namespace TronAksaSharp.Services.Calculators
{
    public class TronFeeCalculatorService
    {
        private readonly TronClient _tronClient;
        private readonly TronGridService _tronGridService;
        private readonly TronNetwork _network;

        public TronFeeCalculatorService(TronClient tronClient , TronGridService tronGridService , TronNetwork tronNetwork)
        {
            _tronClient = tronClient;
            _tronGridService = tronGridService;
            _network = tronNetwork;
        }

        // TRX transferi için tahmini fee hesaplar
        public async Task<decimal>CalculateTRXFeeAsync(string address)
        {
            var feeParams = await _tronGridService.GetTRXFeeParametersAsync();

            // Hesap bilgilerini al 
            var accountInfo = await TronClient.GetBalancesAsync(address, _network); 

            // Bandwidth varsa düşük fee
            if (accountInfo.BandwidthStake > 0)
            {
                return feeParams.TransactionFee * 0.01m;
            }

            // Bandwidth yoksa normal fee
            return feeParams.TransactionFee;
        }
    }
}
