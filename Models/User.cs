namespace RaceDay.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;    
    }
}
