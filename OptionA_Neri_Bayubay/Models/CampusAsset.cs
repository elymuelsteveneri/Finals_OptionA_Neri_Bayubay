using System.Text.Json.Serialization;

namespace OptionA_Neri_Bayubay.Models
{
    public abstract class CampusAsset
    {
        // These are private so the outside world cannot change them directly.
        private decimal purchasePrice;
        private AssetStatus status;

        public string AssetTag { get; set; }
        public string Name { get; set; }
        public DateTime PurchaseDate { get; set; }

        // A property with a simple check.
        public decimal PurchasePrice
        {
            get { return purchasePrice; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Price cannot be negative.");
                }
                purchasePrice = value;
            }
        }

        // Status can only be READ from outside.
        public AssetStatus Status
        {
            get { return status; }
        }

        // Normal constructor.
        public CampusAsset(string tag, string name, DateTime purchaseDate, decimal price)
        {
            AssetTag = tag;
            Name = name;
            PurchaseDate = purchaseDate;
            PurchasePrice = price;
            status = AssetStatus.Available;
        }

        // Empty constructor (for child classes that need it).
        public CampusAsset()
        {
            AssetTag = "";
            Name = "";
            status = AssetStatus.Available;
        }

        // ---- Methods that change status safely ----

        public void MarkInRepair()
        {
            if (status == AssetStatus.CheckedOut)
            {
                throw new Exception("Cannot send to repair: asset is checked out.");
            }
            if (status == AssetStatus.Retired)
            {
                throw new Exception("Cannot send to repair: asset is retired.");
            }
            status = AssetStatus.InRepair;
        }

        public void MarkAvailable()
        {
            if (status == AssetStatus.CheckedOut)
            {
                throw new Exception("Cannot mark available: asset is checked out.");
            }
            status = AssetStatus.Available;
        }

        public void Retire()
        {
            status = AssetStatus.Retired;
        }

        // These two are used by the borrowing classes.
        public void SetCheckedOut()
        {
            status = AssetStatus.CheckedOut;
        }

        public void SetReturned()
        {
            status = AssetStatus.Available;
        }

        // ---- These two methods are different for every asset type ----

        public abstract string GetMaintenanceStatus();

        public abstract decimal CalculateDepreciation();

        // A simple text version of the asset.
        public override string ToString()
        {
            return AssetTag + " - " + Name +
                   " | Price: " + PurchasePrice +
                   " | Status: " + Status;
        }
    }
}
