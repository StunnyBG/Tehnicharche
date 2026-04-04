using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TehnicharcheDbContext context;
        protected readonly DbSet<T> dbSet;

        public GenericRepository(TehnicharcheDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await dbSet.AsNoTracking().ToListAsync();

        public async Task<T?> GetByIdAsync(int id)
            => await dbSet.FindAsync(id);

        public async Task AddAsync(T entity)
            => await dbSet.AddAsync(entity);

        public Task HardDeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
            => await dbSet.AnyAsync(predicate);

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}