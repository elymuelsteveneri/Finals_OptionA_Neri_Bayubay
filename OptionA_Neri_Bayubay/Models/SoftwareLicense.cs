namespace OptionA_Neri_Bayubay.Models
{
    public class SoftwareLicense : CampusAsset
    {
        public string Vendor { get; set; }
        public int SeatCount { get; set; }
        public DateTime SubscriptionEndDate { get; set; }

        public SoftwareLicense() : base()
        {
            Vendor = "";
        }

        public SoftwareLicense(string tag, string name, DateTime purchaseDate,
                               decimal price, string vendor, int seats, DateTime endDate)
            : base(tag, name, purchaseDate, price)
        {
            Vendor = vendor;
            SeatCount = seats;
            SubscriptionEndDate = endDate;
        }

        public override string GetMaintenanceStatus()
        {
            double monthsLeft = (SubscriptionEndDate - DateTime.Today).TotalDays / 30.0;
            if (monthsLeft <= 0)
            {
                return "Expired";
            }
            if (monthsLeft <= 3)
            {
                return "Renew soon";
            }
            return "Active";
        }

        public override decimal CalculateDepreciation()
        {
            double totalMonths = (SubscriptionEndDate - PurchaseDate).TotalDays / 30.0;
            double monthsLeft = (SubscriptionEndDate - DateTime.Today).TotalDays / 30.0;

            if (monthsLeft < 0)
            {
                monthsLeft = 0;
            }
            if (totalMonths <= 0)
            {
                totalMonths = 1;
            }

            double percentLeft = monthsLeft / totalMonths;
            return (decimal)(percentLeft * (double)PurchasePrice);
        }
    }
}
