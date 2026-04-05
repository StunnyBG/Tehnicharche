using Tehnicharche.ViewModels.Listing;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface ISavedListingService
    {
        Task ToggleSaveAsync(string userId, int listingId);

        Task<bool> IsSavedAsync(string userId, int listingId);

        Task<SavedListingsQueryModel> GetSavedListingsAsync(SavedListingsQueryModel query, string userId);
    }
}
