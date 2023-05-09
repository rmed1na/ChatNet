namespace ChatNet.Utils.DateTime
{
    public static class DateTimeUtility
    {
        public static string FullFormat => "dd MMM yyyy HH:mm:ss";

        /// <summary>
        /// Gives back the date in a standard full format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToFullFormat(this System.DateTime value)
            => value.ToString(FullFormat);
    }
}