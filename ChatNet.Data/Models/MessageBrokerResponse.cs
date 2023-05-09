namespace ChatNet.Data.Models
{
    public class MessageBrokerResponse
    {
        public string Response { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public bool Success { get; set; }
    }
}