using ChatNet.Data.Models;
using ChatNet.Data.Models.Constants;
using ChatNet.Data.Models.Settings;
using ChatNet.Data.Repositories;
using ChatNet.Utils.Chats;
using ChatNet.Utils.Identity;
using ChatNet.Utils.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ChatNet.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IModel _messageBroker;
        private readonly IHubContext<ChatHub> _hubCtx;

        /// <summary>
        /// Chatroom group name builder (i.e: chatroom:1, chatroom:2)
        /// </summary>
        /// <param name="roomId">The chatroom identifier</param>
        /// <returns></returns>
        private static string ChatroomGroup(int roomId) => $"chatroom:{roomId}";

        public ChatHub(
            IHubContext<ChatHub> hubCtx, 
            IChatRepository chatRepo, 
            IUserRepository userRepo,
            IOptions<AppSettings> options)
        {
            _hubCtx = hubCtx;
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            var settings = options.Value;
            _messageBroker = new ConnectionFactory
            {
                HostName = settings.MessageBroker.Server,
                UserName = settings.MessageBroker.User,
                Password = settings.MessageBroker.Password
            }
            .CreateConnection()
            .CreateModel();

            _messageBroker.QueueDeclare(
                queue: MessageBrokerParams.REQUEST_QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _messageBroker.QueueDeclare(
                queue: MessageBrokerParams.RESPONSE_QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_messageBroker);
            consumer.Received += async (sender, args) =>
            {
                await SendBotMessageAsync(sender, args);
            };

            _messageBroker.BasicConsume(
                queue: MessageBrokerParams.RESPONSE_QUEUE_NAME,
                autoAck: true,
                consumer: consumer);
        }

        /// <summary>
        /// Sends a message to a group of users inside a chatroom
        /// </summary>
        /// <param name="message">Message content</param>
        /// <param name="roomId">Chatroom identifier</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public async Task SendMessage(string message, int roomId)
        {
            try
            {
                var userData = GetUserData();
                var user = await _userRepo.GetAsync(userData.Username) ?? throw new InvalidDataException("User not found");
                var room = await GetChatRoomAsync(roomId);
                var post = new ChatRoomPost
                {
                    ChatRoomId = room.ChatRoomId,
                    UserId = user.UserId,
                    Owner = user,
                    Message = message
                };

                if (message.StartsWith('/'))
                    SendStockQuoteRequest(message, roomId);
                else
                    await _chatRepo.AddPostAsync(post);

                await Clients
                    .Group(ChatroomGroup(roomId))
                    .SendAsync("NewMessageReceived", ChatDisplayUtility.BuildMessage(post));
            }
            catch (Exception ex)
            {
                await SendException(ex);
            }
        }

        /// <summary>
        /// Sends a message to a group of users inside a chatroom, as a bot (external user)
        /// </summary>
        /// <param name="sender">Caller object</param>
        /// <param name="args">Caller arguments</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public async Task SendBotMessageAsync(object? sender, BasicDeliverEventArgs args)
        {
            if (_hubCtx == null)
                return;

            try
            {
                var responseJson = Encoding.UTF8.GetString(args.Body.ToArray());
                var response = JsonSerializer.Deserialize<MessageBrokerResponse>(responseJson) ?? throw new InvalidCastException("Can't deserialize message broker response (is null).");
                var post = new ChatRoomPost
                {
                    CreatedDate = DateTime.Now,
                    Message = response.Response
                };
                
                await _hubCtx.Clients
                    .Group(ChatroomGroup(response.RoomId))
                    .SendAsync("NewMessageReceived", ChatDisplayUtility.BuildMessage(post, "[StockBot]"));
            }
            catch (Exception ex)
            {
                _ = ex; // debugging
                throw;
            }
        }

        /// <summary>
        /// Subcribes a user into a chatroom to receive it's messages
        /// </summary>
        /// <param name="roomId">Chatroom identifier</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task SubscribeToChatroom(int roomId)
        {
            var room = await _chatRepo.GetRoomAsync(roomId) ?? throw new InvalidOperationException("Chatroom not found. Can't subscribe to it");
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatroomGroup(room.ChatRoomId));
        }

        /// <summary>
        /// Sends exception messages (in case they happen) into the chatroom silently (via console.log on the clients)
        /// for better & easier debugging
        /// </summary>
        /// <param name="ex">The exception to be sent</param>
        /// <returns></returns>
        public async Task SendException(Exception ex)
            => await Clients.Caller.SendAsync("HubException", ex.ToString());

        #region Helpers
        /// <summary>
        /// Gets the user data from the requesting user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private IdentityUserData GetUserData()
            => IdentityUtility.GetIdentityUserData(Context.User?.Identity) ?? throw new InvalidDataException("User data not found");

        /// <summary>
        /// Gets the chatroom by it's identifier
        /// </summary>
        /// <param name="roomId">Chatroom identifier</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private async Task<ChatRoom> GetChatRoomAsync(int roomId)
        {
            var room = await _chatRepo.GetRoomAsync(roomId);
            return room ?? throw new InvalidDataException("Chat room not found");
        }

        /// <summary>
        /// Creates a new request for the chatbot to go and get a stock quote
        /// </summary>
        /// <param name="command">The command user for the stock (i.e.: /stock=aapl.us)</param>
        /// <param name="roomId">Chatroom identifier</param>
        private void SendStockQuoteRequest(string command, int roomId)
        {
            var requestJson = new MessageBrokerRequest
            {
                Command = command,
                RoomId = roomId
            }.ToJson();

            ArgumentException.ThrowIfNullOrEmpty(requestJson);
            var bytes = Encoding.UTF8.GetBytes(requestJson);

            _messageBroker.BasicPublish(
                exchange: string.Empty,
                routingKey: MessageBrokerParams.REQUEST_QUEUE_NAME,
                basicProperties: null,
                body: bytes);
        }
        #endregion
    }
}