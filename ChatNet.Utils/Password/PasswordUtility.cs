using Crypt = BCrypt.Net.BCrypt;

namespace ChatNet.Utils.Password
{
    public static class PasswordUtility
    {
        public static string HashPassword(string password)
            => Crypt.HashPassword(password);

        public static bool VerifyPassword(string raw, string hashed)
            => Crypt.Verify(raw, hashed);
    }
}