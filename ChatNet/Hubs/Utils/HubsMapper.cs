namespace ChatNet.Hubs.Utils
{
    public static class HubsMapper
    {
        public static void MapHubs(this WebApplication app)
        {
            app.MapHub<ChatHub>("/chatHub");
        }
    }
}
