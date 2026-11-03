using OptionA_Neri_Bayubay.Interfaces;
using System.Text.Json.Serialization;

namespace OptionA_Neri_Bayubay.Models
{
    public class NetworkingHardware : CampusAsset, INetworkable
    {
        public string DeviceModel { get; set; }
        public int PortCount { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }

        public NetworkingHardware() : base()
        {
            DeviceModel = "";
            IpAddress = "";
            MacAddress = "";
        }

        public NetworkingHardware(string tag, string name, DateTime purchaseDate,
                                  decimal price, string model, int ports)
            : base(tag, name, purchaseDate, price)
        {
            DeviceModel = model;
            PortCount = ports;
            IpAddress = "";
            MacAddress = "";
        }

        public override string GetMaintenanceStatus()
        {
            double ageInYears = (DateTime.Today - PurchaseDate).TotalDays / 365.0;
            if (ageInYears >= 7)
            {
                return "End-of-life";
            }
            return "Working";
        }

        public override decimal CalculateDepreciation()
        {
            double ageInYears = (DateTime.Today - PurchaseDate).TotalDays / 365.0;
            double percentLeft = 1.0 - (ageInYears / 7.0);
            if (percentLeft < 0)
            {
                percentLeft = 0;
            }
            return (decimal)(percentLeft * (double)PurchasePrice);
        }

        public bool HasNetworkIdentity()
        {
            bool hasIp = !string.IsNullOrEmpty(IpAddress);
            bool hasMac = !string.IsNullOrEmpty(MacAddress);
            return hasIp && hasMac;
        }
    }
}
