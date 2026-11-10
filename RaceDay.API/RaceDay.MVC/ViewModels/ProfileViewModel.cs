using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

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

        public string Role { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public string? OrganisationName { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}