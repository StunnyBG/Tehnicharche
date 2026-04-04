using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class AdminListingService : IAdminListingService
    {
        private readonly IAdminListingRepository listingRepository;
        private readonly ILogger<AdminListingService> logger;

        public AdminListingService(
            IAdminListingRepository listingRepository,
            ILogger<AdminListingService> logger)
        {
            this.listingRepository = listingRepository;
            this.logger = logger;
        }

        public async Task<AdminListingsViewModel> GetListingsAsync(string filter, string? searchTerm, int page)
        {
            page = page <= 0 ? 1 : page;

            var (items, filteredTotal) = await listingRepository.GetAdminFilteredAsync(
                filter, searchTerm, page, AdminPageSize);

            int activeCount = await listingRepository.GetActiveCountAsync();
            int deletedCount = await listingRepository.GetDeletedCountAsync();

            int totalPages = (int)Math.Ceiling((double)filteredTotal / AdminPageSize);
            if (totalPages < 1) totalPages = 1;

            return new AdminListingsViewModel
            {
                Filter = filter,
                SearchTerm = searchTerm,
                ActiveCount = activeCount,
                DeletedCount = deletedCount,
                TotalCount = activeCount + deletedCount,
                Page = page,
                TotalPages = totalPages,
                Listings = items.Select(l => new AdminListingRowViewModel
                {
                    Id = l.Id,
                    Title = l.Title,
                    CreatorName = l.Creator.UserName!,
                    CategoryName = l.Category.Name,
                    Price = l.Price,
                    IsDeleted = l.IsDeleted,
                    CreatedAt = l.CreatedAt.ToString(DateFormat)
                })
            };
        }

        public async Task<IEnumerable<AdminListingRowViewModel>> GetRecentAsync(int count)
        {
            var listings = await listingRepository.GetRecentAdminAsync(count);

            return listings.Select(l => new AdminListingRowViewModel
            {
                Id = l.Id,
                Title = l.Title,
                CreatorName = l.Creator.UserName!,
                CategoryName = l.Category.Name,
                Price = l.Price,
                IsDeleted = l.IsDeleted,
                CreatedAt = l.CreatedAt.ToString(DateFormat)
            });
        }

        public async Task SoftDeleteAsync(int id)
        {
            var listing = await GetActiveOrThrowAsync(id);
            await listingRepository.SoftDeleteAsync(listing);
            logger.LogInformation("Listing {ListingId} soft-deleted by admin.", id);
        }

        public async Task RestoreAsync(int id)
        {
            var listing = await listingRepository.GetByIdDeletedAsync(id)
                ?? throw new InvalidOperationException($"Listing {id} not found.");

            listing.IsDeleted = false;
            await listingRepository.SaveChangesAsync();
            logger.LogInformation("Listing {ListingId} restored by admin.", id);
        }

        public async Task HardDeleteAsync(int id)
        {
            var listing = await listingRepository.GetByIdDeletedAsync(id)
                ?? throw new InvalidOperationException($"Listing {id} not found.");

            await listingRepository.HardDeleteAsync(listing);
            logger.LogInformation("Listing {ListingId} permanently deleted by admin.", id);
        }

        public async Task SoftDeleteAllByUserAsync(string userId)
        {
            await listingRepository.SoftDeleteAllByUserAsync(userId);
            logger.LogInformation("All listings for user {UserId} soft-deleted by admin.", userId);
        }

        // helper
        private async Task<Listing> GetActiveOrThrowAsync(int id)
            => await listingRepository.GetByIdTrackedAsync(id)
               ?? throw new InvalidOperationException($"Listing {id} not found.");
    }
}