using ChatNet.Data.Models;

namespace ChatNet.Data.Repositories
{
    public interface IChatRepository
    {
        /// <summary>
        /// Adds a new chatroom to the database
        /// </summary>
        /// <param name="room">The chatroom object</param>
        /// <returns></returns>
        Task AddRoomAsync(ChatRoom room);

        /// <summary>
        /// Adds a new chatroom post to the database
        /// </summary>
        /// <param name="post">The chatroom post object</param>
        /// <returns></returns>
        Task AddPostAsync(ChatRoomPost post);

        /// <summary>
        /// Gets all the available chatrooms on the database
        /// </summary>
        /// <returns></returns>
        Task<ICollection<ChatRoom>> GetRoomsAsync();

        /// <summary>
        /// Gets a specific chatroom from the database (by it's identifier)
        /// </summary>
        /// <param name="roomId">The chatroom identifier</param>
        /// <returns></returns>
        Task<ChatRoom?> GetRoomAsync(int roomId);

        /// <summary>
        /// Get latest posts from a chatroom
        /// </summary>
        /// <param name="roomId">The chatroom identifier</param>
        /// <param name="take">Amount of posts to retrieve from the database</param>
        /// <returns></returns>
        Task<ICollection<ChatRoomPost>> GetLatestPostsAsync(int roomId, int take);

        /// <summary>
        /// Tells if a chatroom name already exists or not
        /// </summary>
        /// <param name="name">New chatroom name to be checked</param>
        /// <returns></returns>
        Task<bool> RoomNameExistsAsync(string name);
    }
}