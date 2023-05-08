using ChatNet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Data.Context
{
    public interface IChatNetContext : IDisposable
    {
        #region Data set
        DbSet<User> Users { get; }
        DbSet<ChatRoom> ChatRooms { get; }
        #endregion

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
