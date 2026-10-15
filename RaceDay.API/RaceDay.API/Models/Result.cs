namespace RaceDay.API.Models
{
    public class Result
    {
        public int result_id { get; set; }

        public int enrolment_id { get; set; }

        public TimeSpan finish_time { get; set; }

        public int? position { get; set; }

        public string result_status { get; set; } = "Finished";

        public Enrolment? Enrolment { get; set; }
    }
}