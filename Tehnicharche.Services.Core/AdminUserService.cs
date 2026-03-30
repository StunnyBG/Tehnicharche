using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;


namespace Tehnicharche.Services.Core
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAdminListingRepository listingRepository;
        private readonly IContactMessageRepository messageRepository;
        private readonly IGenericRepository<Category> categoryRepository;

        public AdminUserService(
            UserManager<ApplicationUser> userManager,
            IAdminListingRepository listingRepository,
            IContactMessageRepository messageRepository,
            IGenericRepository<Category> categoryRepository)
        {
            this.userManager = userManager;
            this.listingRepository = listingRepository;
            this.messageRepository = messageRepository;
            this.categoryRepository = categoryRepository;
        }

        public async Task<AdminUsersViewModel> GetUsersAsync()
        {
            var users = await userManager.Users.OrderBy(u => u.UserName).ToListAsync();
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
                    Roles = roles
                });
            }

            return new AdminUsersViewModel
            {
                Users = rows,
                TotalCount = rows.Count,
                BannedCount = rows.Count(r => r.IsBanned)
            };
        }

        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            int activeCount = await listingRepository.GetActiveCountAsync();
            int deletedCount = await listingRepository.GetDeletedCountAsync();
            int totalUsers = await userManager.Users.CountAsync();
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
                    CreatedAt = l.CreatedAt.ToString(DateFormat)
                }),
                RecentMessages = recentMessages.Select(m => new AdminMessageRowViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Subject = m.Subject,
                    Message = m.Message,
                    IsRead = m.IsRead,
                    SentAt = m.SentAt.ToString(DateFormat)
                })
            };
        }

        public async Task ToggleRoleAsync(string userId, string role, string currentAdminId)
        {
            if (userId == currentAdminId && role == "Admin")
                throw new InvalidOperationException("You cannot remove your own Admin role.");

            var user = await FindOrThrowAsync(userId);

            if (await userManager.IsInRoleAsync(user, role))
                await userManager.RemoveFromRoleAsync(user, role);
            else
                await userManager.AddToRoleAsync(user, role);
        }

        public async Task BanAsync(string userId)
        {
            var user = await FindOrThrowAsync(userId);

            if (await userManager.IsInRoleAsync(user, "Admin"))
                throw new InvalidOperationException("Administrators cannot be banned. Revoke the Admin role first.");

            user.IsBanned = true;
            await userManager.UpdateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);
            await listingRepository.SoftDeleteAllByUserAsync(userId);
        }

        public async Task UnbanAsync(string userId)
        {
            var user = await FindOrThrowAsync(userId);
            user.IsBanned = false;
            await userManager.UpdateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);
        }


        // helper methods
        private async Task<ApplicationUser> FindOrThrowAsync(string userId)
            => await userManager.FindByIdAsync(userId)
               ?? throw new InvalidOperationException($"User {userId} not found.");
    }
}
