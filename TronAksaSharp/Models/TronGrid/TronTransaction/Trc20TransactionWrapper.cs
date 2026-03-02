    using System.Text.Json.Serialization;

    namespace TronAksaSharp.Models.TronGrid.TronTransaction
    {
        public class Trc20TransactionWrapper
        {
            [JsonPropertyName("data")]
            public List<Trc20Transaction> Data { get; set; }

            [JsonPropertyName("success")]
            public bool Success { get; set; }
        }
    }
