using ChatNet.Data.Models;

namespace ChatNet.Data.Repositories
{
    public interface IChatRepository
    {
        Task AddRoomAsync(ChatRoom room);
        Task AddPostAsync(ChatRoomPost post);
        Task<ICollection<ChatRoom>> GetRoomsAsync();
        Task<ChatRoom?> GetRoomAsync(int roomId);
        Task<ICollection<ChatRoomPost>> GetLatestPostsAsync(int roomId, int take);
        Task<bool> RoomNameExistsAsync(string name);
    }
}