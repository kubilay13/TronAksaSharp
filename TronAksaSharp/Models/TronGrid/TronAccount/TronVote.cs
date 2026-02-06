using System.Text.Json.Serialization;

namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronVote
    {
        // Oy verilen Super Representative adresi
        [JsonPropertyName("vote_address")]
        public string VoteAddress { get; set; }

        // Kullanılan oy sayısı
        [JsonPropertyName("vote_count")]
        public long VoteCount { get; set; }
    }
}
