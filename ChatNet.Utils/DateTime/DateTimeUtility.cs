namespace ChatNet.Utils.DateTime
{
    public static class DateTimeUtility
    {
        public static string FullFormat => "dd MMM yyyy HH:mm:ss";

        public static string ToFullFormat(this System.DateTime value)
            => value.ToString(FullFormat);
    }
}