
using Tehnicharche.ViewModels;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IListingService
    {
        Task<IEnumerable<ListingIndexViewModel>> GetAllListingsAsync(string? userId);

        Task<ListingIndexViewModel?> GetListingByIdAsync(int id);

        Task<IEnumerable<ListingIndexViewModel>> GetListingsByUserAsync(string userId);

        Task<ListingDetailsViewModel> GetListingDetailsByIdAsync(int id);

        Task<ListingCreateViewModel> GetListingCreateViewModelAsync();

        Task AddListingAsync(ListingCreateViewModel model, string creatorId);

        Task<ListingEditViewModel> GetListingEditAsync(int id, string userId);

        Task EditListingAsync(ListingEditViewModel model, string userId);

        Task DeleteListingAsync(int id, string userId);

        Task<ListingDeleteViewModel> GetListingDeleteDetailsAsync(int id, string userId);

        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync();
        
        Task<IEnumerable<CityViewModel>> GetAllCitiesAsync();

        Task<IEnumerable<RegionViewModel>> GetAllRegionsAsync();
    }
}
