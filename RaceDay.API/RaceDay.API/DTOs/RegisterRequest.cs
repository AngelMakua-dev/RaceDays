
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        // Required when registering an organiser.
        [StringLength(100)]
        public string? OrganisationName { get; set; }

        // Required when registering a participant.
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }
    }
}