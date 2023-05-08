using ChatNet.Data.Context.Configuration;
using ChatNet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Data.Context
{
    public class ChatNetContext : DbContext, IChatNetContext
    {
        #region Data sets
        public DbSet<User> Users => Set<User>();
        public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
        public DbSet<ChatRoomPost> ChatRoomPosts => Set<ChatRoomPost>();
        #endregion

        public ChatNetContext() { }
        public ChatNetContext(DbContextOptions<ChatNetContext> options) : base(options) { }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelConfiguration.Apply(modelBuilder);
        }
    }
}