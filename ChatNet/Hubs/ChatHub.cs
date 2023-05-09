using ChatNet.Data.Models;
using ChatNet.Data.Repositories;
using ChatNet.Utils.Chats;
using ChatNet.Utils.DateTime;
using ChatNet.Utils.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;

        private static string ChatroomGroup(int roomId) => $"chatroom:{roomId}";

        public ChatHub(IChatRepository chatRepo, IUserRepository userRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
        }

        public async Task SendMessage(string message, int roomId)
        {
            try
            {
                var userData = GetUserData();
                var user = await _userRepo.GetAsync(userData.Username) ?? throw new InvalidDataException("User not found");
                var room = await GetChatRoomAsync(roomId);
                var timestamp = DateTime.Now.ToFullFormat();
                var post = new ChatRoomPost
                {
                    ChatRoomId = room.ChatRoomId,
                    UserId = user.UserId,
                    Owner = user,
                    Message = message
                };

                await _chatRepo.AddPostAsync(post);
                await Clients
                    .Group(ChatroomGroup(roomId))
                    .SendAsync("NewMessageReceived", userData.Username, timestamp, ChatDisplayUtility.BuildMessage(post));
            }
            catch (Exception ex)
            {
                await SendException(ex);
            }
        }

        public async Task SubscribeToChatroom(int roomId)
        {
            var room = await _chatRepo.GetRoomAsync(roomId) ?? throw new InvalidOperationException("Chatroom not found. Can't subscribe to it");
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatroomGroup(room.ChatRoomId));
        }

        public async Task SendException(Exception ex)
            => await Clients.Caller.SendAsync("HubException", ex.ToString());

        #region Helpers
        private IdentityUserData GetUserData()
            => IdentityUtility.GetIdentityUserData(Context.User?.Identity) ?? throw new InvalidDataException("User data not found");

        private async Task<ChatRoom> GetChatRoomAsync(int roomId)
        {
            var room = await _chatRepo.GetRoomAsync(roomId);
            return room ?? throw new InvalidDataException("Chat room not found"); ;
        }
        #endregion
    }
}