
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CategoryRequest
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999.99")]
        public decimal DistanceKm { get; set; }

        [Range(typeof(decimal), "0", "999999.99")]
        public decimal EntryFee { get; set; }
    }
}