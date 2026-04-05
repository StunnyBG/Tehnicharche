using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Listing;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class SavedListingService : ISavedListingService
    {
        private readonly ISavedListingRepository savedListingRepository;
        private readonly ILogger<SavedListingService> logger;

        public SavedListingService(
            ISavedListingRepository savedListingRepository,
            ILogger<SavedListingService> logger)
        {
            this.savedListingRepository = savedListingRepository;
            this.logger = logger;
        }

        public async Task ToggleSaveAsync(string userId, int listingId)
        {
            if (await savedListingRepository.IsSavedAsync(userId, listingId))
            {
                await savedListingRepository.UnsaveAsync(userId, listingId);
                logger.LogInformation(
                    "User {UserId} unsaved listing {ListingId}.", userId, listingId);
            }
            else
            {
                await savedListingRepository.SaveAsync(userId, listingId);
                logger.LogInformation(
                    "User {UserId} saved listing {ListingId}.", userId, listingId);
            }
        }

        public async Task<bool> IsSavedAsync(string userId, int listingId)
            => await savedListingRepository.IsSavedAsync(userId, listingId);

        public async Task<SavedListingsQueryModel> GetSavedListingsAsync(
            SavedListingsQueryModel query, string userId)
        {
            query.Page = query.Page <= 0 ? DefaultPage : query.Page;

            var (items, total) = await savedListingRepository.GetSavedByUserPagedAsync(
                userId, query.Page, MyListingsPageSize, query.SearchTerm);

            query.TotalListings = total;
            query.Listings = items.Select(l => new ListingIndexViewModel
            {
                Id = l.Id,
                Title = l.Title,
                Price = l.Price.ToString(),
                CategoryName = l.Category.Name,
                RegionName = l.Region.Name,
                CityName = l.City?.Name,
                ImageUrl = l.ImageUrl
            }).ToList();

            return query;
        }
    }
}