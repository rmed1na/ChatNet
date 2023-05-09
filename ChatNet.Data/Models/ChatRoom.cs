using ChatNet.Data.Models.Enums;
using ChatNet.Data.Models.Metadata;

namespace ChatNet.Data.Models
{
    public class ChatRoom : ChatNetModel
    {
        public int ChatRoomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public GlobalStatusCode StatusCode { get; set; }

        public ChatRoom()
        {
            StatusCode = GlobalStatusCode.Active;
        }
    }
}