using Tehnicharche.Data.Models;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface IAdminCityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();

        Task<City?> GetByIdAsync(int id);

        Task<Dictionary<int, int>> GetListingCountsAsync();

        Task<bool> NameExistsInRegionAsync(string name, int regionId, int? excludeId = null);

        Task<bool> IsInUseAsync(int id);

        Task AddAsync(City city);

        Task DeleteAsync(City city);

        Task SaveChangesAsync();
    }
}
