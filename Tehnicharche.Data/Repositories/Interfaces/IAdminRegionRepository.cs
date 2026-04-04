using Tehnicharche.Data.Models;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface IAdminRegionRepository
    {
        Task<IEnumerable<Region>> GetAllAsync();

        Task<Region?> GetByIdAsync(int id);

        Task<Dictionary<int, int>> GetListingCountsAsync();

        Task<Dictionary<int, int>> GetCityCountsAsync();

        Task<bool> NameExistsAsync(string name, int? excludeId = null);

        Task<bool> IsInUseAsync(int id);

        Task AddAsync(Region region);

        Task DeleteAsync(Region region);

        Task SaveChangesAsync();
    }
}
