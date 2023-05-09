namespace ChatNet.Data.Models.Settings
{
    public class ServiceSettings
    {
        public MessageBroker MessageBroker { get; set; }
        public string? StockApiUrl { get; set; }

        public ServiceSettings()

        {
            MessageBroker = new MessageBroker();
        }
    }
}