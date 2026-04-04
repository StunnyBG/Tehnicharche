using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class AdminCategoryRepository : IAdminCategoryRepository
    {
        private readonly TehnicharcheDbContext context;

        public AdminCategoryRepository(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Dictionary<int, int>> GetListingCountsAsync()
            => await context.Listings
                .IgnoreQueryFilters()
                .GroupBy(l => l.CategoryId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
            => await context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower()
                               && (excludeId == null || c.Id != excludeId));

        public async Task<bool> IsInUseAsync(int id)
            => await context.Listings
                .IgnoreQueryFilters()
                .AnyAsync(l => l.CategoryId == id);

        public async Task AddAsync(Category category)
            => await context.Categories.AddAsync(category);

        public async Task DeleteAsync(Category category)
            => context.Categories.Remove(category);

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}