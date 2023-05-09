using ChatNet.Data.Models.Metadata;

namespace ChatNet.Data.Models
{
    public class ChatRoomPost : ChatNetModel
    {
        public int ChatRoomPostId { get; set; }
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;

        public virtual User? Owner { get; set; }
    }
}