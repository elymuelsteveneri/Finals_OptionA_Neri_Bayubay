using System;
using System.ComponentModel.DataAnnotations;

namespace OptionA_Neri_Bayubay.Models
{
    public class AssetUpdateRequest
    {
        [Required]
        public string Name { get; set; } = "";

        public DateTime PurchaseDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; }
    }
}
