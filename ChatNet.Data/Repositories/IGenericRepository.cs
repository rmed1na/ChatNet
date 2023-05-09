namespace ChatNet.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Adds a generic type entity to the database
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns></returns>
        Task AddAsync(T entity);
    }
}