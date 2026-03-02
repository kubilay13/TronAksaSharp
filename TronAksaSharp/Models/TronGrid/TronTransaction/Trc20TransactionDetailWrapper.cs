using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronTransaction
{
    public class Trc20TransactionDetailWrapper
    {
        [JsonPropertyName("data")]
        public List<Trc20TransactionDetail> Data { get; set; }
    }

    public class Trc20TransactionDetail
    {
        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("ret")]
        public List<Trc20TransactionRet> Ret { get; set; }

        [JsonPropertyName("fee")]
        public long Fee { get; set; }
    
        [JsonPropertyName("raw_data")]
        public Trc20RawData RawData { get; set; }
    }

    public class Trc20TransactionRet
    {
        [JsonPropertyName("contractRet")]
        public string ContractRet { get; set; }
    }

    public class Trc20RawData
    {
        [JsonPropertyName("contract")]
        public List<Trc20Contract> Contract { get; set; }
    }

    public class Trc20Contract
    {
        [JsonPropertyName("parameter")]
        public Trc20Parameter Parameter { get; set; }
    }

    public class Trc20Parameter
    {
        [JsonPropertyName("value")]
        public Trc20Value Value { get; set; }
    }

    public class Trc20Value
    {
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
    }
}
