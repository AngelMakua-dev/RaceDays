using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}