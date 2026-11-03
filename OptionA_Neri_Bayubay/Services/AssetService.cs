using OptionA_Neri_Bayubay.Interfaces;
using OptionA_Neri_Bayubay.Models;
using System.Collections.Generic;
using System.Linq;

namespace OptionA_Neri_Bayubay.Models;

/// <summary>
/// In-memory repository + business logic for campus assets.
/// Uses a Dictionary for O(1) lookup by asset tag and exposes LINQ-driven queries.
/// </summary>
public class AssetService
{
    // A Dictionary stores assets using their tag as the key.
    private Dictionary<string, CampusAsset> assets;

    public AssetService()
    {
        assets = new Dictionary<string, CampusAsset>();
    }

    // ---- Basic operations ----

    public bool Add(CampusAsset asset)
    {
        if (asset == null)
        {
            return false;
        }
        if (assets.ContainsKey(asset.AssetTag))
        {
            return false; // tag already used
        }
        assets.Add(asset.AssetTag, asset);
        return true;
    }

    public CampusAsset Get(string tag)
    {
        if (assets.ContainsKey(tag))
        {
            return assets[tag];
        }
        return null;
    }

    public List<CampusAsset> GetAll()
    {
        List<CampusAsset> result = new List<CampusAsset>();
        foreach (CampusAsset asset in assets.Values)
        {
            result.Add(asset);
        }
        return result;
    }

    public bool Remove(string tag)
    {
        if (!assets.ContainsKey(tag))
        {
            return false;
        }
        assets.Remove(tag);
        return true;
    }

    // ---- Borrowing ----

    public string CheckOut(string tag, string borrowerId)
    {
        CampusAsset asset = Get(tag);
        if (asset == null)
        {
            return "Asset not found.";
        }

        // Try to treat this asset as borrowable.
        IBorrowable borrowable = asset as IBorrowable;
        if (borrowable == null)
        {
            return "This asset cannot be borrowed.";
        }

        try
        {
            borrowable.CheckOut(borrowerId);
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string Return(string tag)
    {
        CampusAsset asset = Get(tag);
        if (asset == null)
        {
            return "Asset not found.";
        }

        IBorrowable borrowable = asset as IBorrowable;
        if (borrowable == null)
        {
            return "This asset cannot be borrowed.";
        }

        try
        {
            borrowable.Return();
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    // ---- Queries ----

    // All assets that are currently checked out.
    public List<IBorrowable> GetCheckedOut()
    {
        List<IBorrowable> result = new List<IBorrowable>();
        foreach (CampusAsset asset in assets.Values)
        {
            IBorrowable borrowable = asset as IBorrowable;
            if (borrowable != null && borrowable.IsCheckedOut)
            {
                result.Add(borrowable);
            }
        }
        return result;
    }

    // All assets whose name or tag contains the search text.
    public List<CampusAsset> Search(string text)
    {
        List<CampusAsset> result = new List<CampusAsset>();
        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        string lowerText = text.ToLower();
        foreach (CampusAsset asset in assets.Values)
        {
            bool nameMatches = asset.Name.ToLower().Contains(lowerText);
            bool tagMatches = asset.AssetTag.ToLower().Contains(lowerText);
            if (nameMatches || tagMatches)
            {
                result.Add(asset);
            }
        }
        return result;
    }

    // All assets of one type, for example "Workstation".
    public List<CampusAsset> GetByType(string typeName)
    {
        List<CampusAsset> result = new List<CampusAsset>();
        foreach (CampusAsset asset in assets.Values)
        {
            if (asset.GetType().Name == typeName)
            {
                result.Add(asset);
            }
        }
        return result;
    }

    // All assets currently in repair.
    public List<CampusAsset> GetInRepair()
    {
        List<CampusAsset> result = new List<CampusAsset>();
        foreach (CampusAsset asset in assets.Values)
        {
            if (asset.Status == AssetStatus.InRepair)
            {
                result.Add(asset);
            }
        }
        return result;
    }

    // Network devices that are missing an IP or MAC.
    public List<INetworkable> GetNetworkProblems()
    {
        List<INetworkable> result = new List<INetworkable>();
        foreach (CampusAsset asset in assets.Values)
        {
            INetworkable networkable = asset as INetworkable;
            if (networkable != null && !networkable.HasNetworkIdentity())
            {
                result.Add(networkable);    
            }
        }
        return result;
    }

    // Total current value of everything after depreciation.
    public decimal GetTotalValue()
    {
        decimal total = 0;
        foreach (CampusAsset asset in assets.Values)
        {
            total = total + asset.CalculateDepreciation();
        }
        return total;
    }
}