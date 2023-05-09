using ChatNet.Data.Context;
using ChatNet.Data.Repositories;

namespace ChatNet
{
    /// <summary>
    /// Handles dependency injection for the web application
    /// </summary>
    public static class DependencyInjection
    {
        public static void Configure(IServiceCollection services)
        {
            #region Context
            services.AddScoped<IChatNetContext, ChatNetContext>();
            #endregion

            #region Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            #endregion

        }
    }
}