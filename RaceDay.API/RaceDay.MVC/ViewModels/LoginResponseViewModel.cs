namespace RaceDay.MVC.ViewModels
{
    public class LoginResponseViewModel
    {
        public string Message { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}