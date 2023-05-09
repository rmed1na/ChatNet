namespace ChatNet.Data.Models
{
    public class MessageBrokerRequest
    {
        public string Command { get; set; } = string.Empty;
        public int RoomId { get; set; }
    }
}