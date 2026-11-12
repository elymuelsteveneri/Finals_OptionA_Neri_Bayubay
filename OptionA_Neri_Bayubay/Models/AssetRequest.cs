
using System.ComponentModel.DataAnnotations;

namespace OptionA_Neri_Bayubay.Models
{
    public class AssetRequest
    {
        [Required]
        public string AssetType { get; set; } = "";

        [Required]
        public string AssetTag { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        public DateTime PurchaseDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; }

        // Workstation
        public string Cpu { get; set; } = "";

        [Range(0, int.MaxValue)]
        public int RamGb { get; set; }

        public string LabRoom { get; set; } = "";

        // NetworkingHardware
        public string DeviceModel { get; set; } = "";

        [Range(0, int.MaxValue)]
        public int PortCount { get; set; }

        // Peripheral
        public string DeviceType { get; set; } = "";

        // SoftwareLicense
        public string Vendor { get; set; } = "";

        [Range(0, int.MaxValue)]
        public int SeatCount { get; set; }

        public DateTime SubscriptionEndDate { get; set; }

        // Network information
        public string IpAddress { get; set; } = "";
        public string MacAddress { get; set; } = "";
    }
}
