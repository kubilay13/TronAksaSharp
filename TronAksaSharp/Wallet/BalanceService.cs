using TronAksaSharp.Enums;

namespace TronAksaSharp.Wallet
{
    public class BalanceService
    {

        /// <summary>
        ///  TRX BAKİYESİNİ SORGULAR
        ///  </summary>

        public static async Task<decimal> GetTRXBalanceAsync(string address ,TronNetwork tronNetwork)
        { 
           if(string.IsNullOrWhiteSpace(address))
           {
                throw new ArgumentException("Addres boş olamaz");
           }
           string baseUrl = TronEndpoints.GetBaseUrl(tronNetwork);

           using var client = new HttpClient();
           var json = await client.GetStringAsync($"{baseUrl}/v1/accounts/{address}");

           var document = System.Text.Json.JsonDocument.Parse(json);
           if (!document.RootElement.TryGetProperty("data", out var data)  || data.GetArrayLength() == 0)
           { 
              return 0;
           }
           var balance = data[0].GetProperty("balance").GetDecimal();
           return balance / 1_000_000; // TRX birimi 1 TRX = 1,000,000 sunucudur.
        }
    }
}
