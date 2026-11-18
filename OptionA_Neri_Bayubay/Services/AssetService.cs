using OptionA_Neri_Bayubay.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace OptionA_Neri_Bayubay.Models
{
    public class AssetService
    {
        // Stores assets using their asset tag.
        private Dictionary<string, CampusAsset> assets;

        // Location of the JSON file.
        private readonly string filePath;

        // JSON settings.
        private readonly JsonSerializerOptions jsonOptions;

        public AssetService(IHostEnvironment environment)
        {
            assets = new Dictionary<string, CampusAsset>(
                StringComparer.OrdinalIgnoreCase);

            filePath = Path.Combine(
                environment.ContentRootPath,
                "assets.json");

            jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            LoadFromFile();
        }

        // ──────────────────────────────────────────────
        // LOAD ASSETS FROM JSON FILE
        // ──────────────────────────────────────────────

        private void LoadFromFile()
        {
            // If the file does not exist yet,
            // start with an empty asset collection.
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);

                List<CampusAsset> loadedAssets =
                    JsonSerializer.Deserialize<List<CampusAsset>>(
                        json, jsonOptions);

                if (loadedAssets == null)
                {
                    return;
                }

                foreach (CampusAsset asset in loadedAssets)
                {
                    if (asset == null ||
                        string.IsNullOrWhiteSpace(asset.AssetTag))
                    {
                        throw new InvalidDataException(
                            "The JSON file contains an invalid asset.");
                    }

                    if (assets.ContainsKey(asset.AssetTag))
                    {
                        throw new InvalidDataException(
                            "Duplicate asset tag found in the JSON file: "
                            + asset.AssetTag);
                    }

                    assets.Add(asset.AssetTag, asset);
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "The assets.json file contains invalid JSON. "
                    + "Please check the file before restarting the backend.",
                    ex);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException(
                    "Unable to read the assets.json file: " + ex.Message,
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException(
                    "Access to the assets.json file was denied: "
                    + ex.Message,
                    ex);
            }
        }

        // ──────────────────────────────────────────────
        // SAVE ASSETS TO JSON FILE
        // ──────────────────────────────────────────────

        public void Save()
        {
            string temporaryPath = filePath + ".tmp";

            try
            {
                string json = JsonSerializer.Serialize(
                    assets.Values.ToList(),
                    jsonOptions);

                // Create the folder if it does not exist.
                Directory.CreateDirectory(
                    Path.GetDirectoryName(filePath)!);

                // Write to a temporary file first.
                File.WriteAllText(temporaryPath, json);

                // Replace the old file with the new one.
                File.Move(temporaryPath, filePath, true);
            }
            catch (Exception ex)
            {
                // Remove the temporary file if saving fails.
                if (File.Exists(temporaryPath))
                {
                    try
                    {
                        File.Delete(temporaryPath);
                    }
                    catch
                    {
                        // Ignore cleanup errors.
                    }
                }

                throw new InvalidOperationException(
                    "Unable to save assets to the JSON file: "
                    + ex.Message,
                    ex);
            }
        }

        // ──────────────────────────────────────────────
        // BASIC OPERATIONS
        // ──────────────────────────────────────────────

        public bool Add(CampusAsset asset)
        {
            if (asset == null ||
                string.IsNullOrWhiteSpace(asset.AssetTag))
            {
                return false;
            }

            if (assets.ContainsKey(asset.AssetTag))
            {
                return false;
            }

            assets.Add(asset.AssetTag, asset);

            Save();

            return true;
        }

        public CampusAsset Get(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return null;

            if (assets.ContainsKey(tag.Trim()))
            {
                return assets[tag.Trim()];
            }

            return null;
        }

        public List<CampusAsset> GetAll()
        {
            return assets.Values.ToList();
        }

        public bool Remove(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            bool removed = assets.Remove(tag.Trim());

            if (removed)
            {
                Save();
            }

            return removed;
        }

        // ──────────────────────────────────────────────
        // BORROWING
        // ──────────────────────────────────────────────

        public string CheckOut(string tag, string borrowerId)
        {
            CampusAsset asset = Get(tag);

            if (asset == null)
                return "Asset not found.";

            IBorrowable borrowable = asset as IBorrowable;

            if (borrowable == null)
                return "This asset cannot be borrowed.";

            try
            {
                borrowable.CheckOut(borrowerId);

                Save();

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
                return "Asset not found.";

            IBorrowable borrowable = asset as IBorrowable;

            if (borrowable == null)
                return "This asset cannot be borrowed.";

            try
            {
                borrowable.Return();

                Save();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // ──────────────────────────────────────────────
        // SEARCH ASSETS USING LINQ
        // ──────────────────────────────────────────────

        public List<CampusAsset> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<CampusAsset>();

            return assets.Values
                .Where(asset =>
                    asset.Name.Contains(
                        text.Trim(),
                        StringComparison.OrdinalIgnoreCase) ||
                    asset.AssetTag.Contains(
                        text.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // ──────────────────────────────────────────────
        // FILTER ASSETS BY TYPE USING LINQ
        // ──────────────────────────────────────────────

        public List<CampusAsset> GetByType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return new List<CampusAsset>();

            return assets.Values
                .Where(asset =>
                    asset.GetType().Name.Equals(
                        typeName.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // ──────────────────────────────────────────────
        // FILTER ASSETS BY STATUS USING LINQ
        // ──────────────────────────────────────────────

        public List<CampusAsset> GetByStatus(AssetStatus status)
        {
            return assets.Values
                .Where(asset => asset.Status == status)
                .ToList();
        }

        // ==========================================
        // FILTER ASSETS IN REPAIR
        // ==========================================

        public List<CampusAsset> GetInRepair()
        {
            return GetByStatus(AssetStatus.InRepair);
        }

        // ──────────────────────────────────────────────
        // INTERFACE CASTING: BORROWABLE ASSETS
        // ──────────────────────────────────────────────

        public List<IBorrowable> GetBorrowable()
        {
            return assets.Values
                .OfType<IBorrowable>()
                .ToList();
        }

        // ──────────────────────────────────────────────
        // INTERFACE CASTING: CHECKED-OUT ASSETS
        // ──────────────────────────────────────────────

        public List<IBorrowable> GetCheckedOut()
        {
            return assets.Values
                .OfType<IBorrowable>()
                .Where(asset => asset.IsCheckedOut)
                .ToList();
        }

        // ──────────────────────────────────────────────
        // INTERFACE CASTING: NETWORKABLE ASSETS
        // ──────────────────────────────────────────────

        public List<INetworkable> GetNetworkable()
        {
            return assets.Values
                .OfType<INetworkable>()
                .ToList();
        }

        // ──────────────────────────────────────────────
        // NETWORK DEVICES WITH MISSING IP OR MAC
        // ──────────────────────────────────────────────

        public List<INetworkable> GetNetworkProblems()
        {
            return assets.Values
                .OfType<INetworkable>()
                .Where(asset => !asset.HasNetworkIdentity())
                .ToList();
        }

        // ──────────────────────────────────────────────
        // POLYMORPHISM: TOTAL ASSET VALUE
        // ──────────────────────────────────────────────

        public decimal GetTotalValue()
        {
            return assets.Values
                .Sum(asset => asset.CalculateDepreciation());
        }
    }
}
