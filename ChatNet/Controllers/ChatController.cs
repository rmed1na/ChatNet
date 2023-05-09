using ChatNet.Data.Models;
using ChatNet.Data.Repositories;
using ChatNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatRepository _chatRepo;

        public ChatController(IChatRepository chatRepo)
            => _chatRepo = chatRepo;

        [HttpGet]
        [Route("rooms")]
        public async Task<IActionResult> ChatRooms()
        {
            var rooms = await _chatRepo.GetRoomsAsync();
            return View("~/Views/Chat/Rooms.cshtml", rooms);
        }

        [HttpGet]
        [Route("room/{roomId:int}")]
        public async Task<IActionResult> Chat(int roomId)
        {
            var room = await _chatRepo.GetRoomAsync(roomId);
            if (room == null)
                return NotFound("Chatroom not found");

            var posts = await _chatRepo.GetLatestPostsAsync(roomId, 50);
            var viewModel = new ChatRoomViewModel
            {
                Id = room.ChatRoomId,
                Name = room.Name,
                LatestPosts = posts
            };

            return View("~/Views/Chat/Chat.cshtml", viewModel);
        }

        [HttpGet]
        [Route("room/new")]
        public IActionResult NewChatRoom()
            => View("~/Views/Chat/NewRoom.cshtml");

        [HttpPost]
        [Route("room/new")]
        public async Task<IActionResult> NewChatRoom(ChatRoomViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return BadRequest("Chatroom must have a name");

            var alreadyExists = await _chatRepo.RoomNameExistsAsync(model.Name);
            if (alreadyExists)
                return Conflict($"Chatroom with name {model.Name} already exists");

            var room = new ChatRoom
            {
                Name = model.Name
            };

            await _chatRepo.AddRoomAsync(room);
            return Redirect("/chat/rooms");
        }
    }
}