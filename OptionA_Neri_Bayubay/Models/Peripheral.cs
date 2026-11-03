using OptionA_Neri_Bayubay.Interfaces;

namespace OptionA_Neri_Bayubay.Models
{
    public class Peripheral : CampusAsset, IBorrowable
    {
        public string DeviceType { get; set; }
        public string BorrowerId { get; set; }
        public DateTime? CheckoutDate { get; set; }

        public Peripheral() : base()
        {
            DeviceType = "";
            BorrowerId = null;
        }

        public Peripheral(string tag, string name, DateTime purchaseDate,
                          decimal price, string type)
            : base(tag, name, purchaseDate, price)
        {
            DeviceType = type;
            BorrowerId = null;
        }

        public override string GetMaintenanceStatus()
        {
            double ageInYears = (DateTime.Today - PurchaseDate).TotalDays / 365.0;
            if (ageInYears >= 3)
            {
                return "Check condition (over 3 years old)";
            }
            return "Good";
        }

        public override decimal CalculateDepreciation()
        {
            double ageInYears = (DateTime.Today - PurchaseDate).TotalDays / 365.0;
            double percentLeft = 1.0 - (ageInYears / 3.0);
            if (percentLeft < 0)
            {
                percentLeft = 0;
            }
            return (decimal)(percentLeft * (double)PurchasePrice);
        }

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
                throw new Exception("This peripheral is not checked out.");
            }
            BorrowerId = null;
            CheckoutDate = null;
            SetReturned();
        }
    }
}
