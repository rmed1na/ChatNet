namespace ChatNet.Hubs.Utils
{
    /// <summary>
    /// Mapper for SignalR hubs at the program startup
    /// </summary>
    public static class HubsMapper
    {
        /// <summary>
        /// Maps signalR hubs
        /// </summary>
        /// <param name="app"></param>
        public static void MapHubs(this WebApplication app)
        {
            app.MapHub<ChatHub>("/chatHub");
        }
    }
}
