using ChatNet.Data.Context;
using ChatNet.Data.Repositories;

namespace ChatNet
{
    public static class DependencyInjection
    {
        public static void Configure(IServiceCollection services)
        {
            #region Scoped
            services.AddScoped<IChatNetContext, ChatNetContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            #endregion
        }
    }
}