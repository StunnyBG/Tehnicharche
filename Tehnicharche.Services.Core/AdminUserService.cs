using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserManagerWrapper userManager;
        private readonly IAdminListingRepository listingRepository;
        private readonly IContactMessageRepository messageRepository;
        private readonly IGenericRepository<Category> categoryRepository;
        private readonly ILogger<AdminUserService> logger;

        public AdminUserService(
            IUserManagerWrapper userManager,
            IAdminListingRepository listingRepository,
            IContactMessageRepository messageRepository,
            IGenericRepository<Category> categoryRepository,
            ILogger<AdminUserService> logger)
        {
            this.userManager = userManager;
            this.listingRepository = listingRepository;
            this.messageRepository = messageRepository;
            this.categoryRepository = categoryRepository;
            this.logger = logger;
        }

        public async Task<AdminUsersViewModel> GetUsersAsync(int page = 1, string? searchTerm = null)
        {
            page = page <= 0 ? 1 : page;

            int totalCount = await userManager.CountAsync(searchTerm);
            int bannedCount = await userManager.CountBannedAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / AdminPageSize);
            if (totalPages < 1) totalPages = 1;

            var users = await userManager.GetUsersAsync(
                searchTerm,
                skip: (page - 1) * AdminPageSize,
                take: AdminPageSize);

            var listingCounts = await listingRepository.GetListingCountsByCreatorsAsync();

            var rows = new List<AdminUserRowViewModel>();
            foreach (var u in users)
            {
                var roles = await userManager.GetRolesAsync(u);
                listingCounts.TryGetValue(u.Id, out int count);
                rows.Add(new AdminUserRowViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber,
                    ListingCount = count,
                    IsBanned = u.IsBanned,
                    Roles = roles,
                });
            }

            return new AdminUsersViewModel
            {
                Users = rows,
                TotalCount = totalCount,
                BannedCount = bannedCount,
                Page = page,
                TotalPages = totalPages,
                SearchTerm = searchTerm,
            };
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

        public async Task ToggleRoleAsync(string userId, string role, string currentAdminId)
        {
            if (userId == currentAdminId && role == AdminRole)
                throw new InvalidOperationException("You cannot remove your own Admin role.");

            var user = await FindOrThrowAsync(userId);

            if (await userManager.IsInRoleAsync(user, role))
            {
                await userManager.RemoveFromRoleAsync(user, role);
                logger.LogInformation(
                    "Role '{Role}' removed from user {UserId} by admin {AdminId}.",
                    role, userId, currentAdminId);
            }
            else
            {
                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation(
                    "Role '{Role}' added to user {UserId} by admin {AdminId}.",
                    role, userId, currentAdminId);
            }
        }

        public async Task BanAsync(string userId)
        {
            var user = await FindOrThrowAsync(userId);

            if (await userManager.IsInRoleAsync(user, AdminRole))
                throw new InvalidOperationException(
                    "Administrators cannot be banned. Revoke the Admin role first.");

            user.IsBanned = true;
            await userManager.UpdateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);
            await listingRepository.SoftDeleteAllByUserAsync(userId);

            logger.LogWarning("User {UserId} has been banned by admin.", userId);
        }

        public async Task UnbanAsync(string userId)
        {
            var user = await FindOrThrowAsync(userId);
            user.IsBanned = false;
            await userManager.UpdateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);

            logger.LogInformation("User {UserId} has been unbanned by admin.", userId);
        }

        // helper
        private async Task<ApplicationUser> FindOrThrowAsync(string userId)
            => await userManager.FindByIdAsync(userId)
               ?? throw new InvalidOperationException($"User {userId} not found.");
    }
}