
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

        public void Display()
        {
            int choice = 0;

            while (choice != 6)
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
                Console.WriteLine("║[6] Exit                        ║");
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
                        Console.WriteLine("\nThank you for using the system!");
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice. Please select 1-6.");
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

            Console.WriteLine("\n");
            Console.WriteLine("╔════════════════════════════╗");
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

            // Common asset information
            asset.AssetTag = ReadRequired("\nAsset Tag: ");
            asset.Name = ReadRequired("Asset Name: ");

            asset.PurchaseDate =
                ReadDate("Purchase Date (yyyy-MM-dd): ");

            asset.PurchasePrice =
                ReadDecimal("Purchase Price: ");

            // Workstation information
            if (asset.AssetType == "Workstation")
            {
                asset.Cpu = ReadRequired("CPU: ");

                asset.RamGb =
                    ReadPositiveInt("RAM (GB): ");

                asset.LabRoom =
                    ReadRequired("Lab Room: ");

                asset.IpAddress =
                    ReadRequired("IP Address: ");

                asset.MacAddress =
                    ReadRequired("MAC Address: ");
            }

            // Networking Hardware information
            else if (asset.AssetType == "NetworkingHardware")
            {
                asset.DeviceModel =
                    ReadRequired("Device Model: ");

                asset.PortCount =
                    ReadPositiveInt("Port Count: ");

                asset.IpAddress =
                    ReadRequired("IP Address: ");

                asset.MacAddress =
                    ReadRequired("MAC Address: ");
            }

            // Peripheral information
            else if (asset.AssetType == "Peripheral")
            {
                asset.DeviceType =
                    ReadRequired("Device Type: ");
            }

            // Software License information
            else if (asset.AssetType == "SoftwareLicense")
            {
                asset.Vendor =
                    ReadRequired("Vendor: ");

                asset.SeatCount =
                    ReadPositiveInt("Seat Count: ");

                asset.SubscriptionEndDate =
                    ReadDate("Subscription End Date (yyyy-MM-dd): ");
            }

            // Send information to API
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

                if (assets == null || assets.Count == 0)
                {
                    Console.WriteLine("\nNo assets found.");
                }
                else
                {
                    foreach (AssetResponse asset in assets)
                    {
                        DisplayAsset(asset);
                    }
                }
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

                update.Name =
                    ReadRequired("\nNew Asset Name: ");

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

        // DISPLAY ASSET INFORMATION
        private void DisplayAsset(AssetResponse asset)
        {
            Console.WriteLine("\n──────────────────────────────────────────────");
            Console.WriteLine("Asset Type: " + asset.AssetType);
            Console.WriteLine("Asset Tag: " + asset.AssetTag);
            Console.WriteLine("Name: " + asset.Name);
            Console.WriteLine("Purchase Date: " +
                asset.PurchaseDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("Purchase Price: " + asset.PurchasePrice);
            Console.WriteLine("Status: " + asset.Status);
            Console.WriteLine("Maintenance: " + asset.MaintenanceStatus);
            Console.WriteLine("Current Value: " + asset.DepreciatedValue);

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

        // INPUT VALIDATION

        // Makes sure the user enters something
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

        // Validates integer input
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

        // Validates positive whole numbers
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

        // Validates price
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

        // Validates date
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

        // Validates Y or N confirmation
        private string ReadConfirmation(string message)
        {
            while (true)
            {
                Console.Write(message);

                string input = (Console.ReadLine() ?? "").Trim().ToUpper();

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

    // Data sent to the API
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

    // Data received from the API
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
