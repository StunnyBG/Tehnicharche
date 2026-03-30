using Tehnicharche.Data.Models;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface IAdminListingRepository
    {
        Task<(IEnumerable<Listing> Items, int TotalCount)> GetAdminFilteredAsync(
            string filter, string? searchTerm, int page, int pageSize);

        Task<IEnumerable<Listing>> GetRecentAdminAsync(int count);

        Task<int> GetActiveCountAsync();

        Task<int> GetDeletedCountAsync();

        Task<Dictionary<string, int>> GetListingCountsByCreatorsAsync();

        Task SoftDeleteAllByUserAsync(string userId);

        Task<Listing?> GetByIdDeletedAsync(int id);

        Task<Listing?> GetByIdTrackedAsync(int id);

        Task HardDeleteAsync(Listing listing);

        Task SoftDeleteAsync(Listing listing);

        Task SaveChangesAsync();
    }
}
