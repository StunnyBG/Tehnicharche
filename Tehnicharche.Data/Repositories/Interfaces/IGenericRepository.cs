using System.Linq.Expressions;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);

        Task HardDeleteAsync(T entity);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        Task SaveChangesAsync();
    }
}