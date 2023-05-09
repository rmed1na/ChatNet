using ChatNet.Data.Models.Constants;

namespace ChatNet.Utils.Text
{
    /// <summary>
    /// Set of string utilities as extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Tells if a string is a valid command for the stock bot (valid command i.e.: https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv)
        /// </summary>
        /// <param name="text">The string to be analyzed</param>
        /// <returns>If it is a valid command or not</returns>
        public static bool IsValidCommand(this string text, out string reason)
        {
            reason = string.Empty;
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                reason = "Text is empty or null";
                return false;
            }
            if (!HasCommandStructure(text))
            {
                reason = "Doesn't have the correct command structure (/stock=code)";
                return false;
            }

            var parts = text.Split('=');
            if (parts.Length != 2)
            {
                reason = "Is missing the value part";
                return false;
            }

            var command = parts[0].Replace("/", "");
            var value = parts[1];

            if (!MessageBrokerParams.Commands.Any(x => x.ToLower() == command.ToLower()))
            {
                reason = "The provided command isn't expected";
                return false;
            }
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                reason = "The provided value is empty or null";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for mandatory characters needed for a command to exist
        /// </summary>
        /// <param name="value">The string to be analyzed</param>
        /// <returns>If has command structure or not</returns>
        private static bool HasCommandStructure(string value)
            => value.StartsWith('/') && value.Contains('=');
    }
}
