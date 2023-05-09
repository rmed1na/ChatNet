using ChatNet.Data.Context;
using ChatNet.Data.Models;
using ChatNet.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Data.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly IChatNetContext _ctx;

        public ChatRepository(IChatNetContext ctx)
            => _ctx = ctx;

        public async Task AddRoomAsync(ChatRoom room)
        {
            await _ctx.ChatRooms.AddAsync(room);
            await _ctx.SaveChangesAsync();
        }

        public async Task AddPostAsync(ChatRoomPost post)
        {
            await _ctx.ChatRoomPosts.AddAsync(post);
            await _ctx.SaveChangesAsync();
        }

        public async Task<ICollection<ChatRoom>> GetRoomsAsync()
        {
            return await _ctx.ChatRooms
                .Where(x => x.StatusCode == GlobalStatusCode.Active)
                .ToListAsync();
        }

        public async Task<ChatRoom?> GetRoomAsync(int roomId)
            => await _ctx.ChatRooms.FirstOrDefaultAsync(x => x.ChatRoomId == roomId);

        public async Task<ICollection<ChatRoomPost>> GetLatestPostsAsync(int roomId, int take)
        {
            return await _ctx.ChatRoomPosts
                .Where(x => x.ChatRoomId == roomId)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> RoomNameExistsAsync(string name)
            => await _ctx.ChatRooms.AnyAsync(x => x.Name.ToLower() == name.ToLower());
    }
}