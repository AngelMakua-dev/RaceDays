
namespace RaceDay.API.Models
{
    public class Organiser
    {
        public int organiser_id { get; set; }

        public int user_id { get; set; }

        public string organisation_name { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}