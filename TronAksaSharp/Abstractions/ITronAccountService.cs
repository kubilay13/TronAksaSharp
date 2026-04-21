using System.Text.Json;
using TronAksaSharp.Enums;

namespace TronAksaSharp.Abstractions
{
    public interface ITronAccountService
    {
        Task<JsonDocument>GetAccountAsync(string address, TronNetwork network);
    }
}
