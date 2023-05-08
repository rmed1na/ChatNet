namespace ChatNet.Data.Models
{
    public class IdentityUserData
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
    }
}