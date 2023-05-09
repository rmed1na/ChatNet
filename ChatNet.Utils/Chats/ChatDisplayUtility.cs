using ChatNet.Data.Models;
using ChatNet.Utils.DateTime;
using Ganss.Xss;
using System.Text;

namespace ChatNet.Utils.Chats
{
    public static class ChatDisplayUtility
    {
        /// <summary>
        /// Builds the message that will be displayed on the user screen from a post inside the chatroom
        /// </summary>
        /// <param name="post">The chatroom post object</param>
        /// <param name="externalOwnerName">External user (in case the user doesn't have a login)</param>
        /// <returns></returns>
        public static string BuildMessage(ChatRoomPost post, string? externalOwnerName = null)
        {
            var htmlSanitizer = new HtmlSanitizer();
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
                .Append(htmlSanitizer.Sanitize(post.Message));

            return builder.ToString();
        }
    }
}