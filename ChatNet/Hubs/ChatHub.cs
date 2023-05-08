using ChatNet.Data.Repositories;
using ChatNet.Utils.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepo;

        private static string ChatroomGroup(int roomId) => $"chatroom:{roomId}";

        public ChatHub(IChatRepository chatRepo)
            => _chatRepo = chatRepo;

        public async Task SendMessage(string message, int roomId)
        {
            var userData = IdentityUtility.GetIdentityUserData(Context.User?.Identity);
            if (userData == null)
                return;

            var timestamp = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
            await Clients
                .Group(ChatroomGroup(roomId))
                .SendAsync("NewMessageReceived", userData.Username, timestamp, message);
        }

        public async Task SubscribeToChatroom(int roomId)
        {
            var room = await _chatRepo.GetRoomAsync(roomId) ?? throw new InvalidOperationException("Chatroom not found. Can't subscribe to it");
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatroomGroup(room.ChatRoomId));
        }
    }
}