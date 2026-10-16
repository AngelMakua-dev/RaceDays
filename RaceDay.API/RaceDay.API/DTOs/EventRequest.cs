using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class EventRequest
    {
        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999.99")]
        public decimal DistanceKm { get; set; }

        [Required]
        [RegularExpression(
            "^(Run|Walk|Cycle)$",
            ErrorMessage = "Event type must be Run, Walk, or Cycle.")]
        public string EventType { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Upcoming";
    }
}