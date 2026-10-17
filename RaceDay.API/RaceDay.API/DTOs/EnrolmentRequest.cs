
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class EnrolmentRequest
    {
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}