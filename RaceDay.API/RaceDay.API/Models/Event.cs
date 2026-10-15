namespace RaceDay.API.Models
{
    public class Event
    {
        public int event_id { get; set; }

        public int organiser_id { get; set; }

        public string event_name { get; set; } = string.Empty;

        public DateTime event_date { get; set; }

        public string location { get; set; } = string.Empty;

        public string? description { get; set; }

        public decimal distance_km { get; set; }

        public string event_type { get; set; } = string.Empty;

        public string status { get; set; } = "Upcoming";

        public Organiser? Organiser { get; set; }

        public ICollection<Category> Categories { get; set; }
            = new List<Category>();
    }
}