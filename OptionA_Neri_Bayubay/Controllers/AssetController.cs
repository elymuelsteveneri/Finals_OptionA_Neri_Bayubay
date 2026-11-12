
using Microsoft.AspNetCore.Mvc;
using OptionA_Neri_Bayubay.Interfaces;
using OptionA_Neri_Bayubay.Models;

namespace CampusITTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly AssetService service;

        public AssetController(AssetService service)
        {
            this.service = service;
        }

        // GET: api/asset
        [HttpGet]
        public ActionResult<List<AssetResponse>> GetAll()
        {
            List<AssetResponse> result = new List<AssetResponse>();

            foreach (CampusAsset asset in service.GetAll())
            {
                result.Add(ConvertToResponse(asset));
            }

            return Ok(result);
        }

        // GET: api/asset/WS-001
        [HttpGet("{tag}")]
        public ActionResult<AssetResponse> GetByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest("Asset tag is required.");
            }

            CampusAsset asset = service.Get(tag.Trim());

            if (asset == null)
            {
                return NotFound("Asset not found.");
            }

            return Ok(ConvertToResponse(asset));
        }

        // POST: api/asset
        [HttpPost]
        public ActionResult AddAsset(AssetRequest request)
        {
            if (request == null)
            {
                return BadRequest("Asset information is required.");
            }

            // Validate common fields
            if (string.IsNullOrWhiteSpace(request.AssetType))
                return BadRequest("Asset type is required.");

            if (string.IsNullOrWhiteSpace(request.AssetTag))
                return BadRequest("Asset tag is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Asset name is required.");

            if (request.PurchaseDate == DateTime.MinValue)
                return BadRequest("Purchase date is required.");

            if (request.PurchaseDate.Date > DateTime.Today)
                return BadRequest("Purchase date cannot be in the future.");

            if (request.PurchasePrice < 0)
                return BadRequest("Price cannot be negative.");

            string assetType = request.AssetType.Trim().ToLower();

            string assetTag = request.AssetTag.Trim();
            string assetName = request.Name.Trim();

            CampusAsset asset;

            switch (assetType)
            {
                case "workstation":

                    if (string.IsNullOrWhiteSpace(request.Cpu))
                        return BadRequest("CPU is required.");

                    if (request.RamGb <= 0)
                        return BadRequest("RAM must be greater than zero.");

                    if (string.IsNullOrWhiteSpace(request.LabRoom))
                        return BadRequest("Lab room is required.");

                    asset = new Workstation(
                        assetTag,
                        assetName,
                        request.PurchaseDate,
                        request.PurchasePrice,
                        request.Cpu.Trim(),
                        request.RamGb,
                        request.LabRoom.Trim()
                    );

                    ((Workstation)asset).IpAddress = request.IpAddress?.Trim() ?? "";
                    ((Workstation)asset).MacAddress = request.MacAddress?.Trim() ?? "";

                    break;

                case "networkinghardware":

                    if (string.IsNullOrWhiteSpace(request.DeviceModel))
                        return BadRequest("Device model is required.");

                    if (request.PortCount <= 0)
                        return BadRequest("Port count must be greater than zero.");

                    asset = new NetworkingHardware(
                        assetTag,
                        assetName,
                        request.PurchaseDate,
                        request.PurchasePrice,
                        request.DeviceModel.Trim(),
                        request.PortCount
                    );

                    ((NetworkingHardware)asset).IpAddress = request.IpAddress?.Trim() ?? "";
                    ((NetworkingHardware)asset).MacAddress = request.MacAddress?.Trim() ?? "";

                    break;

                case "peripheral":

                    if (string.IsNullOrWhiteSpace(request.DeviceType))
                        return BadRequest("Device type is required.");

                    asset = new Peripheral(
                        assetTag,
                        assetName,
                        request.PurchaseDate,
                        request.PurchasePrice,
                        request.DeviceType.Trim()
                    );

                    break;

                case "softwarelicense":

                    if (string.IsNullOrWhiteSpace(request.Vendor))
                        return BadRequest("Vendor is required.");

                    if (request.SeatCount <= 0)
                        return BadRequest("Seat count must be greater than zero.");

                    if (request.SubscriptionEndDate == DateTime.MinValue)
                        return BadRequest("Subscription end date is required.");

                    if (request.SubscriptionEndDate.Date <= request.PurchaseDate.Date)
                        return BadRequest("Subscription end date must be after purchase date.");

                    asset = new SoftwareLicense(
                        assetTag,
                        assetName,
                        request.PurchaseDate,
                        request.PurchasePrice,
                        request.Vendor.Trim(),
                        request.SeatCount,
                        request.SubscriptionEndDate
                    );

                    break;

                default:
                    return BadRequest("Invalid asset type. Choose Workstation, NetworkingHardware, Peripheral, or SoftwareLicense.");
            }

            if (!service.Add(asset))
            {
                return Conflict("Asset tag already exists.");
            }

            return Ok("Asset added successfully.");
        }

        // PUT: api/asset/WS-001
        [HttpPut("{tag}")]
        public ActionResult UpdateAsset(string tag, AssetRequest request)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return BadRequest("Asset tag is required.");

            CampusAsset asset = service.Get(tag.Trim());

            if (asset == null)
                return NotFound("Asset not found.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Asset name is required.");

            if (request.PurchaseDate == DateTime.MinValue)
                return BadRequest("Purchase date is required.");

            if (request.PurchaseDate.Date > DateTime.Today)
                return BadRequest("Purchase date cannot be in the future.");

            if (request.PurchasePrice < 0)
                return BadRequest("Price cannot be negative.");

            asset.Name = request.Name.Trim();
            asset.PurchaseDate = request.PurchaseDate;
            asset.PurchasePrice = request.PurchasePrice;

            return Ok("Asset updated successfully.");
        }

        // DELETE: api/asset/WS-001
        [HttpDelete("{tag}")]
        public ActionResult DeleteAsset(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return BadRequest("Asset tag is required.");

            CampusAsset asset = service.Get(tag.Trim());

            if (asset == null)
                return NotFound("Asset not found.");

            if (asset is IBorrowable borrowable &&
                borrowable.IsCheckedOut)
            {
                return BadRequest("Cannot delete: asset is checked out.");
            }

            service.Remove(tag.Trim());

            return Ok("Asset deleted successfully.");
        }

        // Convert the different asset classes into one response format.
        private AssetResponse ConvertToResponse(CampusAsset asset)
        {
            AssetResponse response = new AssetResponse();

            response.AssetType = asset.GetType().Name;
            response.AssetTag = asset.AssetTag;
            response.Name = asset.Name;
            response.PurchaseDate = asset.PurchaseDate;
            response.PurchasePrice = asset.PurchasePrice;
            response.Status = asset.Status;
            response.MaintenanceStatus = asset.GetMaintenanceStatus();
            response.DepreciatedValue = asset.CalculateDepreciation();

            if (asset is Workstation workstation)
            {
                response.Cpu = workstation.Cpu;
                response.RamGb = workstation.RamGb;
                response.LabRoom = workstation.LabRoom;
                response.BorrowerId = workstation.BorrowerId ?? "";
                response.CheckoutDate = workstation.CheckoutDate;
                response.IpAddress = workstation.IpAddress;
                response.MacAddress = workstation.MacAddress;
            }
            else if (asset is NetworkingHardware network)
            {
                response.DeviceModel = network.DeviceModel;
                response.PortCount = network.PortCount;
                response.IpAddress = network.IpAddress;
                response.MacAddress = network.MacAddress;
            }
            else if (asset is Peripheral peripheral)
            {
                response.DeviceType = peripheral.DeviceType;
                response.BorrowerId = peripheral.BorrowerId ?? "";
                response.CheckoutDate = peripheral.CheckoutDate;
            }
            else if (asset is SoftwareLicense license)
            {
                response.Vendor = license.Vendor;
                response.SeatCount = license.SeatCount;
                response.SubscriptionEndDate = license.SubscriptionEndDate;
            }

            return response;
        }
    }
}
