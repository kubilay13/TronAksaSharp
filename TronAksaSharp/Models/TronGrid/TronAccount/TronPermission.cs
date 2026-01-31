namespace TronAksaSharp.Models.TronGrid.TronAccount
{
    public class TronPermission
    {
        // owner / active
        public string PermissionName { get; set; }

        // Permission tipi
        public string Type { get; set; }

        // İmza eşiği
        public int Threshold { get; set; }

        // Permission ID
        public int Id { get; set; }

        // Hangi işlemlere izin var
        public string Operations { get; set; }

        // Yetkili adresler
        public List<TronPermissionKey> Keys { get; set; }
    }
}
