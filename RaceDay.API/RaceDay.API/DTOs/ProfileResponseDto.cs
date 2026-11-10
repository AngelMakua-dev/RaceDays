namespace RaceDay.API.DTOs
{
    public class ProfileResponseDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Participant information
        public DateTime? DateOfBirth { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        // Organiser information
        public string? OrganisationName { get; set; }
    }
}