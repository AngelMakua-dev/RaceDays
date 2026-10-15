namespace RaceDay.API.Models
{
    public class Enrolment
    {
        public int enrolment_id { get; set; }

        public int participant_id { get; set; }

        public int category_id { get; set; }

        public DateTime enrolment_date { get; set; }

        public string status { get; set; } = "Registered";

        public Participant? Participant { get; set; }

        public Category? Category { get; set; }

        public Result? Result { get; set; }
    }
}