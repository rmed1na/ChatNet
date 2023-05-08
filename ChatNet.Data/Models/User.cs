using ChatNet.Data.Models.Metadata;

namespace ChatNet.Data.Models
{
    public class User : ChatNetModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Password { get; set; } = string.Empty;

        public string GetFullName()
            => $"{FirstName} {LastName}";
    }
}