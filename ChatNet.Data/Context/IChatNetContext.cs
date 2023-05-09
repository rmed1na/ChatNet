using ChatNet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Data.Context
{
    public interface IChatNetContext : IDisposable
    {
        #region Data set
        DbSet<User> Users { get; }
        DbSet<ChatRoom> ChatRooms { get; }
        DbSet<ChatRoomPost> ChatRoomPosts { get; }
        #endregion

        /// <summary>
        /// Saves changes made to an entity and tracked by EF (that are pending)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes made to an entity and tracked by EF (that are pending)
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}
