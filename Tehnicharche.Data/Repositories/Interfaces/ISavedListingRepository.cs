using Tehnicharche.Data.Models;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface ISavedListingRepository
    {
        Task<bool> IsSavedAsync(string userId, int listingId);

        Task SaveAsync(string userId, int listingId);

        Task UnsaveAsync(string userId, int listingId);

        Task<(IEnumerable<Listing> Items, int TotalCount)> GetSavedByUserPagedAsync(
            string userId, int page, int pageSize, string? searchTerm);

        Task DeleteByListingIdAsync(int listingId);
    }
}
