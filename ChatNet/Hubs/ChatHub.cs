using ChatNet.Utils.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var userData = IdentityUtility.GetIdentityUserData(Context.User?.Identity);
            if (userData == null)
                return;

            var timestamp = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
            await Clients.All.SendAsync(
                "NewMessageReceived",
                userData.Username,
                timestamp,
                message);
        }
    }
}