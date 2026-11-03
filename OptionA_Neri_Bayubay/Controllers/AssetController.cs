using OptionA_Neri_Bayubay.Interfaces;
using OptionA_Neri_Bayubay.Models;

namespace CampusITTracker.Controllers;

// The controller is what the user (console menu) talks to.
// It calls the service and returns simple messages.
public class AssetController
{
    private AssetService service;

    public AssetController(AssetService service)
    {
        this.service = service;
    }

    // ---- CRUD ----

    public string AddAsset(CampusAsset asset)
    {
        if (service.Add(asset))
        {
            return "OK";
        }
        return "Failed to add (tag may already exist).";
    }

    public CampusAsset GetAsset(string tag)
    {
        return service.Get(tag);
    }

    public List<CampusAsset> ListAll()
    {
        return service.GetAll();
    }

    public string DeleteAsset(string tag)
    {
        CampusAsset asset = service.Get(tag);
        if (asset == null)
        {
            return "Asset not found.";
        }

        // Do not allow deleting something that is checked out.
        IBorrowable borrowable = asset as IBorrowable;
        if (borrowable != null && borrowable.IsCheckedOut)
        {
            return "Cannot delete: asset is checked out.";
        }

        service.Remove(tag);
        return "OK";
    }

    // ---- Status ----

    public string SendToRepair(string tag)
    {
        CampusAsset asset = service.Get(tag);
        if (asset == null)
        {
            return "Asset not found.";
        }

        try
        {
            asset.MarkInRepair();
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string MarkRepaired(string tag)
    {
        CampusAsset asset = service.Get(tag);
        if (asset == null)
        {
            return "Asset not found.";
        }

        try
        {
            asset.MarkAvailable();
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    // ---- Borrowing ----

    public string CheckOut(string tag, string borrowerId)
    {
        return service.CheckOut(tag, borrowerId);
    }

    public string Return(string tag)
    {
        return service.Return(tag);
    }

    // ---- Queries ----

    public List<IBorrowable> GetCheckedOut()
    {
        return service.GetCheckedOut();
    }

    public List<CampusAsset> Search(string text)
    {
        return service.Search(text);
    }

    public List<CampusAsset> GetByType(string typeName)
    {
        return service.GetByType(typeName);
    }

    public List<CampusAsset> GetInRepair()
    {
        return service.GetInRepair();
    }

    public List<INetworkable> GetNetworkProblems()
    {
        return service.GetNetworkProblems();
    }

    public decimal GetTotalValue()
    {
        return service.GetTotalValue();
    }
}