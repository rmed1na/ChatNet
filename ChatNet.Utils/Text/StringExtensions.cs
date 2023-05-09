namespace ChatNet.Utils.Text
{
    /// <summary>
    /// Set of string utilities as extensions
    /// </summary>
    public static class StringExtensions
    {
        private static readonly List<string> _commands = new List<string>
        {
            "stock"
        };

        /// <summary>
        /// Tells if a string is a valid command for the stock bot (valid command i.e.: https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv)
        /// </summary>
        /// <param name="text">The string to be analyzed</param>
        /// <returns>If it is a valid command or not</returns>
        public static bool IsValidCommand(this string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return false;
            if (!HasCommandStructure(text))
                return false;

            var parts = text.Split('=');
            if (parts.Length != 2)
                return false;

            var command = parts[0].Replace("/", "");
            var value = parts[1];

            if (!_commands.Any(x => x.ToLower() == command.ToLower()))
                return false;
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return false;

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
