using ChatNet.Data.Models;

namespace ChatNet.Models
{
    public class ChatRoomViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ChatRoomPost>? LatestPosts { get; set; }
    }
}