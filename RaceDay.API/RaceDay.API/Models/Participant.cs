
namespace RaceDay.API.Models
{
    public class Participant
    {
        public int participant_id { get; set; }

        public int user_id { get; set; }

        public DateTime date_of_birth { get; set; }

        public string emergency_contact_name { get; set; } = string.Empty;

        public string emergency_contact_phone { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}