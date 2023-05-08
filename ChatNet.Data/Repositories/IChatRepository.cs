using ChatNet.Data.Models;

namespace ChatNet.Data.Repositories
{
    public interface IChatRepository
    {
        Task AddRoomAsync(ChatRoom room);
        Task<ICollection<ChatRoom>> GetRoomsAsync();
        Task<ChatRoom?> GetRoomAsync(int roomId);
        Task<bool> RoomNameExistsAsync(string name);
    }
}