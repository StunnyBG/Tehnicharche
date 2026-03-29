using Tehnicharche.Data.Models;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface IListingRepository
    {
        Task<(IEnumerable<Listing> Items, int TotalCount)> GetFilteredPagedAsync(
            int page,
            int pageSize,
            int? categoryId,
            int? regionId,
            int? cityId,
            decimal? minPrice,
            decimal? maxPrice,
            string? searchTerm);

        Task<(IEnumerable<Listing> Items, int TotalCount)> GetByCreatorPagedAsync(
            string creatorId,
            int page,
            int pageSize,
            string? searchTerm);

        Task<Listing?> GetByIdWithDetailsAsync(int id);

        Task<Listing?> GetByIdAsync(int id);

        Task<Listing?> GetByIdTrackedAsync(int id);

        Task AddAsync(Listing listing);

        Task SoftDeleteAsync(Listing listing);

        Task SaveChangesAsync();
    }
}
