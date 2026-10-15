namespace RaceDay.API.Models
{
    public class Category
    {
        public int category_id { get; set; }

        public int event_id { get; set; }

        public string category_name { get; set; } = string.Empty;

        public decimal distance_km { get; set; }

        public decimal entry_fee { get; set; }

        public Event? Event { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();
    }
}