using ChatNet.Data.Models;
using ChatNet.Utils.DateTime;
using System.Text;

namespace ChatNet.Utils.Chats
{
    public static class ChatDisplayUtility
    {
        public static string BuildMessage(ChatRoomPost post, string? externalOwnerName = null)
        {
            //TODO: Sanitize message content to avoid code injection
            var builder = new StringBuilder();
            builder
                .Append('[')
                .Append(post.CreatedDate.ToFullFormat())
                .Append("] ");

            if (post.Owner != null)
                builder.Append(post.Owner.Username);
            else if (!string.IsNullOrEmpty(externalOwnerName))
                builder.Append(externalOwnerName);
            else
                builder.Append("?????");

            builder
                .Append(": ")
                .Append(post.Message);

            return builder.ToString();
        }
    }
}