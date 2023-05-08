using ChatNet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Data.Context.Configuration
{
    internal static class ModelConfiguration
    {
        /// <summary>
        /// Applies model configurations (database first approach)
        /// </summary>
        /// <param name="builder">Model builder instance</param>
        public static void Apply(ModelBuilder builder)
        {
            builder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.UserId);
                e.Property(x => x.UserId).UseIdentityColumn();
            });

            builder.Entity<ChatRoom>(e =>
            {
                e.ToTable("ChatRooms");
                e.HasKey(x => x.ChatRoomId);
                e.Property(x => x.ChatRoomId).UseIdentityColumn();
            });

            builder.Entity<ChatRoomPost>(e =>
            {
                e.ToTable("ChatRoomPost");
                e.HasKey(x => x.ChatRoomPostId);
                e.Property(x => x.ChatRoomPostId).UseIdentityColumn();
            });
        }
    }
}