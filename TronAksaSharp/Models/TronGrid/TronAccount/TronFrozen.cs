namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronFrozen
    {
        // Freeze edilen miktar (SUN)
        public long Amount { get; set; }

        // ENERGY / BANDWIDTH / TRON_POWER
        public string Type { get; set; }
    }
}
