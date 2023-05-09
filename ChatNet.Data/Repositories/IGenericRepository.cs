namespace ChatNet.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
    }
}