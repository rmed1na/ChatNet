namespace ChatNet.Data.Models.Constants
{
    public static class MessageBrokerParams
    {
        public static string[] Commands
        {
            get
            {
                return new[]
                {
                    "stock"
                };
            }
        }

        public const string REQUEST_QUEUE_NAME = "ChatNet.Stock.Request";
        public const string RESPONSE_QUEUE_NAME = "ChatNet.Stock.Response";
    }
}