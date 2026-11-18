using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;

namespace Project.UI
{
    public class MainMenu
    {
        private HttpClient client;

        public MainMenu(HttpClient client)
        {
            this.client = client;
        }

        // MAIN MENU
        public void Display()
        {
            int choice = 0;

            while (choice != 10)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════╗");
                Console.WriteLine("║     CAMPUS IT ASSET TRACKER    ║");
                Console.WriteLine("╠════════════════════════════════╣");
                Console.WriteLine("║[1] Add Asset                   ║");
                Console.WriteLine("║[2] View All Assets             ║");
                Console.WriteLine("║[3] View Asset Details          ║");
                Console.WriteLine("║[4] Update Asset                ║");
                Console.WriteLine("║[5] Delete Asset                ║");
                Console.WriteLine("║[6] Check Out Asset             ║");
                Console.WriteLine("║[7] Check In Asset              ║");
                Console.WriteLine("║[8] Search and Filter Assets    ║");
                Console.WriteLine("║[9] View Reports                ║");
                Console.WriteLine("║[10] Exit                       ║");
                Console.WriteLine("╚════════════════════════════════╝");

                choice = ReadInt("Enter choice: ");

                switch (choice)
                {
                    case 1:
                        AddAsset();
                        break;

                    case 2:
                        ViewAllAssets();
                        break;

                    case 3:
                        ViewAssetDetails();
                        break;

                    case 4:
                        UpdateAsset();
                        break;

                    case 5:
                        DeleteAsset();
                        break;

                    case 6:
                        CheckOutAsset();
                        break;

                    case 7:
                        CheckInAsset();
                        break;

                    case 8:
                        SearchAndFilterMenu();
                        break;

                    case 9:
                        ReportsMenu();
                        break;

                    case 10:
                        Console.WriteLine("\nThank you for using the system!");
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice. Please select 1-10.");
                        Pause();
                        break;
                }
            }
        }

        // ADD ASSET
        private void AddAsset()
        {
            Console.Clear();
            Console.WriteLine("═════════ ADD ASSET ═════════");

            AssetRequest asset = new AssetRequest();

            Console.WriteLine("\n╔════════════════════════════╗");
            Console.WriteLine("║     Select Asset Type      ║");
            Console.WriteLine("╠════════════════════════════╣");
            Console.WriteLine("║[1] Workstation             ║");
            Console.WriteLine("║[2] Networking Hardware     ║");
            Console.WriteLine("║[3] Peripheral              ║");
            Console.WriteLine("║[4] Software License        ║");
            Console.WriteLine("╚════════════════════════════╝");

            int type = ReadInt("Enter choice: ");

            switch (type)
            {
                case 1:
                    asset.AssetType = "Workstation";
                    break;

                case 2:
                    asset.AssetType = "NetworkingHardware";
                    break;

                case 3:
                    asset.AssetType = "Peripheral";
                    break;

                case 4:
                    asset.AssetType = "SoftwareLicense";
                    break;

                default:
                    Console.WriteLine("\nInvalid asset type.");
                    Pause();
                    return;
            }

            asset.AssetTag = ReadRequired("\nAsset Tag: ");
            asset.Name = ReadRequired("Asset Name: ");

            asset.PurchaseDate =
                ReadDate("Purchase Date (yyyy-MM-dd): ");

            asset.PurchasePrice =
                ReadDecimal("Purchase Price: ");

            if (asset.AssetType == "Workstation")
            {
                asset.Cpu = ReadRequired("CPU: ");
                asset.RamGb = ReadPositiveInt("RAM (GB): ");
                asset.LabRoom = ReadRequired("Lab Room: ");
                asset.IpAddress = ReadRequired("IP Address: ");
                asset.MacAddress = ReadRequired("MAC Address: ");
            }
            else if (asset.AssetType == "NetworkingHardware")
            {
                asset.DeviceModel = ReadRequired("Device Model: ");
                asset.PortCount = ReadPositiveInt("Port Count: ");
                asset.IpAddress = ReadRequired("IP Address: ");
                asset.MacAddress = ReadRequired("MAC Address: ");
            }
            else if (asset.AssetType == "Peripheral")
            {
                asset.DeviceType = ReadRequired("Device Type: ");
            }
            else if (asset.AssetType == "SoftwareLicense")
            {
                asset.Vendor = ReadRequired("Vendor: ");
                asset.SeatCount = ReadPositiveInt("Seat Count: ");

                asset.SubscriptionEndDate =
                    ReadDate("Subscription End Date (yyyy-MM-dd): ");
            }

            try
            {
                HttpResponseMessage response =
                    client.PostAsJsonAsync("api/asset", asset)
                    .GetAwaiter().GetResult();

                string message =
                    response.Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();

                Console.WriteLine("\n" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nConnection error: " + ex.Message);
            }

            Pause();
        }

        // VIEW ALL ASSETS
        private void ViewAllAssets()
        {
            Console.Clear();
            Console.WriteLine("═════════ ALL ASSETS ═════════");

            try
            {
                List<AssetResponse> assets =
                    client.GetFromJsonAsync<List<AssetResponse>>("api/asset")
                    .GetAwaiter().GetResult()!;

                DisplayAssetList(assets);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nConnection error: " + ex.Message);
            }

            Pause();
        }

        // VIEW ASSET DETAILS
        private void ViewAssetDetails()
        {
            Console.Clear();
            Console.WriteLine("═════════ ASSET DETAILS ═════════");

            string tag = ReadRequired("\nEnter Asset Tag: ");

            try
            {
                AssetResponse asset =
                    client.GetFromJsonAsync<AssetResponse>(
                        "api/asset/" + Uri.EscapeDataString(tag))
                    .GetAwaiter().GetResult()!;

                if (asset == null)
                {
                    Console.WriteLine("\nAsset not found.");
                }
                else
                {
                    DisplayAsset(asset);
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nAsset not found or API unavailable.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
            }

            Pause();
        }

        // UPDATE ASSET
        private void UpdateAsset()
        {
            Console.Clear();
            Console.WriteLine("═════════ UPDATE ASSET ═════════");

            string tag = ReadRequired("\nEnter Asset Tag: ");

            try
            {
                AssetResponse existing =
                    client.GetFromJsonAsync<AssetResponse>(
                        "api/asset/" + Uri.EscapeDataString(tag))
                    .GetAwaiter().GetResult()!;

                if (existing == null)
                {
                    Console.WriteLine("\nAsset not found.");
                    Pause();
                    return;
                }

                Console.WriteLine("\nCurrent Asset Information:");
                DisplayAsset(existing);

                AssetRequest update = new AssetRequest();

                update.Name = ReadRequired("\nNew Asset Name: ");

                update.PurchaseDate =
                    ReadDate("New Purchase Date (yyyy-MM-dd): ");

                update.PurchasePrice =
                    ReadDecimal("New Purchase Price: ");

                HttpResponseMessage response =
                    client.PutAsJsonAsync(
                        "api/asset/" + Uri.EscapeDataString(tag), update)
                    .GetAwaiter().GetResult();

                string message =
                    response.Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();

                Console.WriteLine("\n" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
            }

            Pause();
        }

        // DELETE ASSET
        private void DeleteAsset()
        {
            Console.Clear();
            Console.WriteLine("═════════ DELETE ASSET ═════════");

            string tag = ReadRequired("\nEnter Asset Tag: ");

            string confirm = ReadConfirmation(
                "Are you sure you want to delete this asset? (Y/N): ");

            if (confirm == "N")
            {
                Console.WriteLine("\nDeletion cancelled.");
                Pause();
                return;
            }

            try
            {
                HttpResponseMessage response =
                    client.DeleteAsync(
                        "api/asset/" + Uri.EscapeDataString(tag))
                    .GetAwaiter().GetResult();

                string message =
                    response.Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();

                Console.WriteLine("\n" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nConnection error: " + ex.Message);
            }

            Pause();
        }

        // CHECK OUT ASSET
        private void CheckOutAsset()
        {
            Console.Clear();
            Console.WriteLine("═════════ CHECK OUT ASSET ═════════");

            string tag = ReadRequired("\nEnter Asset Tag: ");
            string borrowerId = ReadRequired("Enter Borrower ID: ");

            try
            {
                var request = new
                {
                    AssetTag = tag,
                    BorrowerId = borrowerId
                };

                HttpResponseMessage response =
                    client.PostAsJsonAsync("api/asset/checkout", request)
                    .GetAwaiter().GetResult();

                string message =
                    response.Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();

                Console.WriteLine("\n" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nConnection error: " + ex.Message);
            }

            Pause();
        }

        // CHECK IN ASSET
        private void CheckInAsset()
        {
            Console.Clear();
            Console.WriteLine("═════════ CHECK IN ASSET ═════════");

            string tag = ReadRequired("\nEnter Asset Tag: ");

            try
            {
                var request = new
                {
                    AssetTag = tag
                };

                HttpResponseMessage response =
                    client.PostAsJsonAsync("api/asset/checkin", request)
                    .GetAwaiter().GetResult();

                string message =
                    response.Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();

                Console.WriteLine("\n" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nConnection error: " + ex.Message);
            }

            Pause();
        }

        // SEARCH AND FILTER MENU
        private void SearchAndFilterMenu()
        {
            int choice = 0;

            while (choice != 4)
            {
                Console.Clear();

                Console.WriteLine("╔════════════════════════════════╗");
                Console.WriteLine("║      SEARCH AND FILTER         ║");
                Console.WriteLine("╠════════════════════════════════╣");
                Console.WriteLine("║[1] Search by Name or Tag       ║");
                Console.WriteLine("║[2] Filter by Asset Type        ║");
                Console.WriteLine("║[3] Filter by Asset Status      ║");
                Console.WriteLine("║[4] Back to Main Menu           ║");
                Console.WriteLine("╚════════════════════════════════╝");

                choice = ReadInt("Enter choice: ");

                switch (choice)
                {
                    case 1:
                        SearchAssets();
                        break;

                    case 2:
                        FilterByType();
                        break;

                    case 3:
                        FilterByStatus();
                        break;

                    case 4:
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice. Please select 1-4.");
                        Pause();
                        break;
                }
            }
        }

        // SEARCH BY NAME OR TAG
        private void SearchAssets()
        {
            Console.Clear();
            Console.WriteLine("═════════ SEARCH ASSETS ═════════");

            string searchText =
                ReadRequired("\nEnter Asset Name or Tag: ");

            try
            {
                List<AssetResponse> assets =
                    client.GetFromJsonAsync<List<AssetResponse>>(
                        "api/asset/search?text=" +
                        Uri.EscapeDataString(searchText))
                    .GetAwaiter().GetResult()!;

                Console.WriteLine("\nSearch Results:");
                DisplayAssetList(assets);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nSearch error: " + ex.Message);
            }

            Pause();
        }

        // FILTER BY ASSET TYPE
        private void FilterByType()
        {
            Console.Clear();

            Console.WriteLine("═════════ FILTER BY TYPE ═════════");
            Console.WriteLine("[1] Workstation");
            Console.WriteLine("[2] Networking Hardware");
            Console.WriteLine("[3] Peripheral");
            Console.WriteLine("[4] Software License");

            int choice = ReadInt("\nSelect Asset Type: ");

            string type;

            switch (choice)
            {
                case 1:
                    type = "Workstation";
                    break;

                case 2:
                    type = "NetworkingHardware";
                    break;

                case 3:
                    type = "Peripheral";
                    break;

                case 4:
                    type = "SoftwareLicense";
                    break;

                default:
                    Console.WriteLine("\nInvalid asset type.");
                    Pause();
                    return;
            }

            try
            {
                List<AssetResponse> assets =
                    client.GetFromJsonAsync<List<AssetResponse>>(
                        "api/asset/type/" + type)
                    .GetAwaiter().GetResult()!;

                Console.WriteLine("\nFiltered Assets:");
                DisplayAssetList(assets);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFilter error: " + ex.Message);
            }

            Pause();
        }

        // FILTER BY ASSET STATUS
        private void FilterByStatus()
        {
            Console.Clear();

            Console.WriteLine("═════════ FILTER BY STATUS ═════════");
            Console.WriteLine("[1] Available");
            Console.WriteLine("[2] Checked Out");
            Console.WriteLine("[3] In Repair");
            Console.WriteLine("[4] Retired");

            int choice = ReadInt("\nSelect Asset Status: ");

            string status;

            switch (choice)
            {
                case 1:
                    status = "Available";
                    break;

                case 2:
                    status = "CheckedOut";
                    break;

                case 3:
                    status = "InRepair";
                    break;

                case 4:
                    status = "Retired";
                    break;

                default:
                    Console.WriteLine("\nInvalid asset status.");
                    Pause();
                    return;
            }

            try
            {
                List<AssetResponse> assets =
                    client.GetFromJsonAsync<List<AssetResponse>>(
                        "api/asset/status/" + status)
                    .GetAwaiter().GetResult()!;

                Console.WriteLine("\nFiltered Assets:");
                DisplayAssetList(assets);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFilter error: " + ex.Message);
            }

            Pause();
        }

        // REPORTS MENU
        private void ReportsMenu()
        {
            int choice = 0;

            while (choice != 3)
            {
                Console.Clear();

                Console.WriteLine("╔════════════════════════════════╗");
                Console.WriteLine("║          ASSET REPORTS         ║");
                Console.WriteLine("╠════════════════════════════════╣");
                Console.WriteLine("║[1] Maintenance Report          ║");
                Console.WriteLine("║[2] Depreciation Report         ║");
                Console.WriteLine("║[3] Back to Main Menu           ║");
                Console.WriteLine("╚════════════════════════════════╝");

                choice = ReadInt("Enter choice: ");

                switch (choice)
                {
                    case 1:
                        ViewMaintenanceReport();
                        break;

                    case 2:
                        ViewDepreciationReport();
                        break;

                    case 3:
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice. Please select 1-3.");
                        Pause();
                        break;
                }
            }
        }

        // MAINTENANCE REPORT
        private void ViewMaintenanceReport()
        {
            Console.Clear();
            Console.WriteLine("═════════ MAINTENANCE REPORT ═════════");

            try
            {
                List<AssetResponse> assets =
                    client.GetFromJsonAsync<List<AssetResponse>>(
                        "api/asset/maintenance")
                    .GetAwaiter().GetResult()!;

                if (assets == null || assets.Count == 0)
                {
                    Console.WriteLine(
                        "\nNo assets currently need maintenance.");
                }
                else
                {
                    Console.WriteLine("\nAssets Needing Maintenance:");
                    DisplayAssetList(assets);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nReport error: " + ex.Message);
            }

            Pause();
        }

        // DEPRECIATION REPORT
        private void ViewDepreciationReport()
        {
            Console.Clear();
            Console.WriteLine("═════════ DEPRECIATION REPORT ═════════");

            try
            {
                List<AssetResponse> assets =
                    client.GetFromJsonAsync<List<AssetResponse>>(
                        "api/asset/depreciation")
                    .GetAwaiter().GetResult()!;

                if (assets == null || assets.Count == 0)
                {
                    Console.WriteLine("\nNo assets available for the report.");
                }
                else
                {
                    decimal totalValue = 0;

                    foreach (AssetResponse asset in assets)
                    {
                        DisplayAsset(asset);
                        totalValue += asset.DepreciatedValue;
                    }

                    Console.WriteLine("\n──────────────────────────────────────────────");
                    Console.WriteLine("Total Current Asset Value: " +
                        totalValue.ToString("N2"));
                    Console.WriteLine("──────────────────────────────────────────────");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nReport error: " + ex.Message);
            }

            Pause();
        }

        // DISPLAY A LIST OF ASSETS
        private void DisplayAssetList(List<AssetResponse> assets)
        {
            if (assets == null || assets.Count == 0)
            {
                Console.WriteLine("\nNo matching assets found.");
                return;
            }

            Console.WriteLine("\nTotal Results: " + assets.Count);

            foreach (AssetResponse asset in assets)
            {
                DisplayAsset(asset);
            }
        }

        // DISPLAY ASSET INFORMATION
        private void DisplayAsset(AssetResponse asset)
        {
            Console.WriteLine("\n──────────────────────────────────────────────");
            Console.WriteLine("Asset Type: " + asset.AssetType);
            Console.WriteLine("Asset Tag: " + asset.AssetTag);
            Console.WriteLine("Name: " + asset.Name);

            Console.WriteLine("Purchase Date: " +
                asset.PurchaseDate.ToString("yyyy-MM-dd"));

            Console.WriteLine("Purchase Price: " +
                asset.PurchasePrice.ToString("N2"));

            Console.WriteLine("Status: " + GetStatusName(asset.Status));
            Console.WriteLine("Maintenance: " + asset.MaintenanceStatus);

            Console.WriteLine("Current Value: " +
                asset.DepreciatedValue.ToString("N2"));

            if (asset.AssetType == "Workstation")
            {
                Console.WriteLine("CPU: " + asset.Cpu);
                Console.WriteLine("RAM: " + asset.RamGb + " GB");
                Console.WriteLine("Lab Room: " + asset.LabRoom);
                Console.WriteLine("Borrower: " + asset.BorrowerId);
                Console.WriteLine("IP Address: " + asset.IpAddress);
                Console.WriteLine("MAC Address: " + asset.MacAddress);
            }
            else if (asset.AssetType == "NetworkingHardware")
            {
                Console.WriteLine("Device Model: " + asset.DeviceModel);
                Console.WriteLine("Port Count: " + asset.PortCount);
                Console.WriteLine("IP Address: " + asset.IpAddress);
                Console.WriteLine("MAC Address: " + asset.MacAddress);
            }
            else if (asset.AssetType == "Peripheral")
            {
                Console.WriteLine("Device Type: " + asset.DeviceType);
                Console.WriteLine("Borrower: " + asset.BorrowerId);
            }
            else if (asset.AssetType == "SoftwareLicense")
            {
                Console.WriteLine("Vendor: " + asset.Vendor);
                Console.WriteLine("Seat Count: " + asset.SeatCount);

                Console.WriteLine("Subscription End Date: " +
                    asset.SubscriptionEndDate.ToString("yyyy-MM-dd"));
            }

            Console.WriteLine("──────────────────────────────────────────────");
        }

        // CONVERT STATUS NUMBER TO NAME
        private string GetStatusName(int status)
        {
            switch (status)
            {
                case 0:
                    return "Available";

                case 1:
                    return "CheckedOut";

                case 2:
                    return "InRepair";

                case 3:
                    return "Retired";

                default:
                    return "Unknown";
            }
        }

        // INPUT VALIDATION
        private string ReadRequired(string message)
        {
            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Input is required. Please try again.");
            }
        }

        private int ReadInt(string message)
        {
            int number;

            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine() ?? "";

                if (int.TryParse(input, out number))
                {
                    return number;
                }

                Console.WriteLine("Please enter a valid number.");
            }
        }

        private int ReadPositiveInt(string message)
        {
            int number;

            while (true)
            {
                number = ReadInt(message);

                if (number > 0)
                {
                    return number;
                }

                Console.WriteLine("Value must be greater than zero.");
            }
        }

        private decimal ReadDecimal(string message)
        {
            decimal number;

            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine() ?? "";

                if (decimal.TryParse(
                    input,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out number) && number >= 0)
                {
                    return number;
                }

                Console.WriteLine(
                    "Enter a valid price. Negative values are not allowed.");
            }
        }

        private DateTime ReadDate(string message)
        {
            DateTime date;

            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Date is required. Please try again.");
                    continue;
                }

                if (DateTime.TryParseExact(
                    input,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
                {
                    if (date.Date <= DateTime.Today)
                    {
                        return date;
                    }

                    Console.WriteLine("Date cannot be in the future.");
                }
                else
                {
                    Console.WriteLine(
                        "Invalid date. Use yyyy-MM-dd (example: 2026-11-12).");
                }
            }
        }

        private string ReadConfirmation(string message)
        {
            while (true)
            {
                Console.Write(message);

                string input =
                    (Console.ReadLine() ?? "").Trim().ToUpper();

                if (input == "Y" || input == "N")
                {
                    return input;
                }

                Console.WriteLine("Please enter Y or N.");
            }
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    // DATA SENT TO THE API
    public class AssetRequest
    {
        public string AssetType { get; set; } = "";
        public string AssetTag { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }

        public string Cpu { get; set; } = "";
        public int RamGb { get; set; }
        public string LabRoom { get; set; } = "";

        public string DeviceModel { get; set; } = "";
        public int PortCount { get; set; }

        public string DeviceType { get; set; } = "";

        public string Vendor { get; set; } = "";
        public int SeatCount { get; set; }
        public DateTime SubscriptionEndDate { get; set; }

        public string IpAddress { get; set; } = "";
        public string MacAddress { get; set; } = "";
    }

    // DATA RECEIVED FROM THE API
    public class AssetResponse
    {
        public string AssetType { get; set; } = "";
        public string AssetTag { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Status { get; set; }

        public string MaintenanceStatus { get; set; } = "";
        public decimal DepreciatedValue { get; set; }

        public string Cpu { get; set; } = "";
        public int RamGb { get; set; }
        public string LabRoom { get; set; } = "";

        public string DeviceModel { get; set; } = "";
        public int PortCount { get; set; }

        public string DeviceType { get; set; } = "";

        public string Vendor { get; set; } = "";
        public int SeatCount { get; set; }
        public DateTime SubscriptionEndDate { get; set; }

        public string BorrowerId { get; set; } = "";
        public DateTime? CheckoutDate { get; set; }

        public string IpAddress { get; set; } = "";
        public string MacAddress { get; set; } = "";
    }
}
