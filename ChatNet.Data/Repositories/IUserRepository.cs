using ChatNet.Data.Models;

namespace ChatNet.Data.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Tells if the username already exists or not on the database
        /// </summary>
        /// <param name="username">Username to be checked</param>
        /// <returns></returns>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Gives back a specific user by it's username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<User?> GetAsync(string username);
    }
}