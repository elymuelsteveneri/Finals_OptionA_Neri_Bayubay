using OptionA_Neri_Bayubay.Interfaces;

namespace OptionA_Neri_Bayubay.Models
{
    public class Workstation : CampusAsset, IBorrowable, INetworkable
    {
        public string Cpu { get; set; }
        public int RamGb { get; set; }
        public string LabRoom { get; set; }

        public string BorrowerId { get; set; }
        public DateTime? CheckoutDate { get; set; }

        public string IpAddress { get; set; }
        public string MacAddress { get; set; }

        // Empty constructor.
        public Workstation() : base()
        {
            Cpu = "";
            LabRoom = "";
            BorrowerId = null;
            IpAddress = "";
            MacAddress = "";
        }

        // Full constructor.
        public Workstation(string tag, string name, DateTime purchaseDate, decimal price,
                           string cpu, int ram, string room)
            : base(tag, name, purchaseDate, price)
        {
            Cpu = cpu;
            RamGb = ram;
            LabRoom = room;
            BorrowerId = null;
            IpAddress = "";
            MacAddress = "";
        }

        // ---- Each type writes its own version of these ----

        public override string GetMaintenanceStatus()
        {
            double ageInYears = (DateTime.Today - PurchaseDate).TotalDays / 365.0;
            if (ageInYears >= 5)
            {
                return "Needs replacement (over 5 years old)";
            }
            return "OK";
        }

        public override decimal CalculateDepreciation()
        {
            double ageInYears = (DateTime.Today - PurchaseDate).TotalDays / 365.0;
            double percentLeft = 1.0 - (ageInYears / 5.0);
            if (percentLeft < 0)
            {
                percentLeft = 0;
            }
            return (decimal)(percentLeft * (double)PurchasePrice);
        }

        // ---- IBorrowable ----

        public bool IsCheckedOut
        {
            get { return BorrowerId != null; }
        }

        public void CheckOut(string borrowerId)
        {
            if (borrowerId == null || borrowerId == "")
            {
                throw new Exception("Borrower ID is required.");
            }
            if (Status == AssetStatus.InRepair)
            {
                throw new Exception("Cannot check out: asset is In Repair.");
            }
            if (Status != AssetStatus.Available)
            {
                throw new Exception("Cannot check out: asset is not available.");
            }
            BorrowerId = borrowerId;
            CheckoutDate = DateTime.Now;
            SetCheckedOut();
        }

        public void Return()
        {
            if (!IsCheckedOut)
            {
                throw new Exception("This workstation is not checked out.");
            }
            BorrowerId = null;
            CheckoutDate = null;
            SetReturned();
        }

        // ---- INetworkable ----

        public bool HasNetworkIdentity()
        {
            bool hasIp = !string.IsNullOrEmpty(IpAddress);
            bool hasMac = !string.IsNullOrEmpty(MacAddress);
            return hasIp && hasMac;
        }
    }
}
