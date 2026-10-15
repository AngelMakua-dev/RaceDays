
namespace RaceDay.API.Models
{
    public class User
    {
        public int user_id { get; set; }

        public string first_name { get; set; } = string.Empty;

        public string last_name { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;

        public string role { get; set; } = string.Empty;

        public DateTime created_at { get; set; }
    }
}
