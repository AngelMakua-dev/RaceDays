using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Participant";

        public string? OrganisationName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }
    }
}