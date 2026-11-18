using System;
using System.Text.Json.Serialization;

namespace OptionA_Neri_Bayubay.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$assetType")]
    [JsonDerivedType(typeof(Workstation), "workstation")]
    [JsonDerivedType(typeof(NetworkingHardware), "networkingHardware")]
    [JsonDerivedType(typeof(Peripheral), "peripheral")]
    [JsonDerivedType(typeof(SoftwareLicense), "softwareLicense")]
    public abstract class CampusAsset
    {
        [JsonInclude]
        public string AssetTag { get; private set; }

        public string Name { get; set; }

        public DateTime PurchaseDate { get; set; }

        private decimal purchasePrice;

        public decimal PurchasePrice
        {
            get { return purchasePrice; }
            set
            {
                if (value < 0)
                    throw new Exception("Price cannot be negative.");

                purchasePrice = value;
            }
        }

        [JsonInclude]
        public AssetStatus Status { get; private set; }

        public CampusAsset(
            string tag,
            string name,
            DateTime purchaseDate,
            decimal price)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new Exception("Asset tag is required.");

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Asset name is required.");

            if (purchaseDate.Date > DateTime.Today)
                throw new Exception("Purchase date cannot be in the future.");

            AssetTag = tag.Trim();
            Name = name.Trim();
            PurchaseDate = purchaseDate;
            PurchasePrice = price;

            Status = AssetStatus.Available;
        }

        // Parameterless constructor for JSON deserialization
        public CampusAsset()
        {
            AssetTag = "";
            Name = "";
            Status = AssetStatus.Available;
        }

        public void MarkInRepair()
        {
            if (Status == AssetStatus.CheckedOut)
                throw new Exception("Cannot send to repair: asset is checked out.");

            if (Status == AssetStatus.Retired)
                throw new Exception("Cannot send to repair: asset is retired.");

            Status = AssetStatus.InRepair;
        }

        public void MarkAvailable()
        {
            if (Status == AssetStatus.CheckedOut)
                throw new Exception("Cannot mark available: asset is checked out.");

            if (Status == AssetStatus.Retired)
                throw new Exception("Cannot mark available: asset is retired.");

            Status = AssetStatus.Available;
        }

        public void Retire()
        {
            if (Status == AssetStatus.CheckedOut)
                throw new Exception("Cannot retire an asset that is checked out.");

            Status = AssetStatus.Retired;
        }

        protected void SetCheckedOut()
        {
            if (Status != AssetStatus.Available)
                throw new Exception("Only available assets can be checked out.");

            Status = AssetStatus.CheckedOut;
        }

        protected void SetReturned()
        {
            if (Status != AssetStatus.CheckedOut)
                throw new Exception("Asset is not currently checked out.");

            Status = AssetStatus.Available;
        }

        public abstract string GetMaintenanceStatus();

        public abstract decimal CalculateDepreciation();

        public override string ToString()
        {
            return AssetTag + " - " + Name +
                   " | Price: " + PurchasePrice +
                   " | Status: " + Status;
        }
    }
}
