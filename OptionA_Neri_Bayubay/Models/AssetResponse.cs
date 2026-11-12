
namespace OptionA_Neri_Bayubay.Models
{
    public class AssetResponse
    {
        public string AssetType { get; set; } = "";
        public string AssetTag { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public AssetStatus Status { get; set; }

        public string MaintenanceStatus { get; set; } = "";
        public decimal DepreciatedValue { get; set; }

        public string Cpu { get; set; } = "";
        public int RamGb { get; set; }
        public string LabRoom { get; set; } = "";

        public string DeviceModel { get; set; } = "";
        public int PortCount { get; set; }

        public string DeviceType { get; set; } = "";

        public string Vendor { get; set; } = "";
        public int SeatCount { get; set; }
        public DateTime SubscriptionEndDate { get; set; }

        public string BorrowerId { get; set; } = "";
        public DateTime? CheckoutDate { get; set; }

        public string IpAddress { get; set; } = "";
        public string MacAddress { get; set; }
            = "";
    }
}
