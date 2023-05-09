using Crypt = BCrypt.Net.BCrypt;

namespace ChatNet.Utils.Password
{
    public static class PasswordUtility
    {
        /// <summary>
        /// Hashes a password to be stored in a more secure way
        /// </summary>
        /// <param name="password">The password string to be hashed</param>
        /// <returns></returns>
        public static string HashPassword(string password)
            => Crypt.HashPassword(password);

        /// <summary>
        /// Verifies if a raw password matches to a hashed one
        /// </summary>
        /// <param name="raw">The raw password to be verified</param>
        /// <param name="hashed">The hashed string to compare agains to</param>
        /// <returns></returns>
        public static bool VerifyPassword(string raw, string hashed)
            => Crypt.Verify(raw, hashed);
    }
}