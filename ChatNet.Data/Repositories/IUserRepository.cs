using ChatNet.Data.Models;

namespace ChatNet.Data.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> UsernameExistsAsync(string username);
        Task<User?> GetAsync(string username);
    }
}