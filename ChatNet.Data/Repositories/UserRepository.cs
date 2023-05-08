using ChatNet.Data.Context;
using ChatNet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IChatNetContext _ctx;

        public UserRepository(IChatNetContext ctx)
            => _ctx = ctx;

        public async Task AddAsync(User entity)
        {
            await _ctx.Users.AddAsync(entity);
            await _ctx.SaveChangesAsync();
        }

        public async Task<User?> GetAsync(string username)
            => await _ctx.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        public async Task<bool> UsernameExistsAsync(string username)
            => await _ctx.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
    }
}