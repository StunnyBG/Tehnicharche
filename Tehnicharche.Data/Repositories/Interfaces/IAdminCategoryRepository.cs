using Tehnicharche.Data.Models;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface IAdminCategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<Dictionary<int, int>> GetListingCountsAsync();

        Task<bool> NameExistsAsync(string name, int? excludeId = null);

        Task<bool> IsInUseAsync(int id);

        Task AddAsync(Category category);

        Task DeleteAsync(Category category);

        Task SaveChangesAsync();
    }
}
