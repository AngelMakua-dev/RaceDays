using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class ResultRequest
    {
        [Range(1, int.MaxValue)]
        public int EnrolmentId { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int FinishingPosition { get; set; }
    }
}