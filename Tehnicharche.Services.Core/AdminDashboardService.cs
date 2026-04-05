using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUserManagerWrapper userManager;
        private readonly IAdminListingRepository listingRepository;
        private readonly IContactMessageRepository messageRepository;
        private readonly IGenericRepository<Category> categoryRepository;

        public AdminDashboardService(
            IUserManagerWrapper userManager,
            IAdminListingRepository listingRepository,
            IContactMessageRepository messageRepository,
            IGenericRepository<Category> categoryRepository)
        {
            this.userManager = userManager;
            this.listingRepository = listingRepository;
            this.messageRepository = messageRepository;
            this.categoryRepository = categoryRepository;
        }

        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            int activeCount = await listingRepository.GetActiveCountAsync();
            int deletedCount = await listingRepository.GetDeletedCountAsync();
            int totalUsers = await userManager.CountAsync(null);
            int totalMsgs = await messageRepository.GetTotalCountAsync();
            int unreadMsgs = await messageRepository.GetUnreadCountAsync();

            var categories = await categoryRepository.GetAllAsync();
            int totalCats = categories.Count();

            var recentListings = await listingRepository.GetRecentAdminAsync(RecentListingsCount);
            var recentMessages = await messageRepository.GetRecentAsync(RecentMessagesCount);

            return new AdminDashboardViewModel
            {
                TotalActiveListings = activeCount,
                TotalDeletedListings = deletedCount,
                TotalUsers = totalUsers,
                TotalMessages = totalMsgs,
                UnreadMessages = unreadMsgs,
                TotalCategories = totalCats,
                RecentListings = recentListings.Select(l => new AdminListingRowViewModel
                {
                    Id = l.Id,
                    Title = l.Title,
                    CreatorName = l.Creator.UserName!,
                    CategoryName = l.Category.Name,
                    Price = l.Price,
                    IsDeleted = l.IsDeleted,
                    CreatedAt = l.CreatedAt.ToString(DateFormat),
                }),
                RecentMessages = recentMessages.Select(m => new AdminMessageRowViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Subject = m.Subject,
                    Message = m.Message,
                    IsRead = m.IsRead,
                    SentAt = m.SentAt.ToString(DateFormat),
                }),
            };
        }
    }
}