namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronPermissionKey
    {
        // Yetkili adres
        public string Address { get; set; }

        // Ağırlık (multi-sig için)
        public int Weight { get; set; }
    }

}
