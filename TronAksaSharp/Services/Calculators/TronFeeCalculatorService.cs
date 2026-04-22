using TronAksaSharp.Enums;
using TronAksaSharp.Wallet;

namespace TronAksaSharp.Services.Calculators
{
    public static class TronFeeCalculatorService
    {
        public static async Task<decimal> CalculateTRXFeeAsync(TronNetwork network, string apiKey = "")
        {
            var tronGridService = new TronGridService(apiKey, network);
            var feeParams = await tronGridService.GetTRXFeeParametersAsync();

            return feeParams.TransactionFee;
        }
    }
}
