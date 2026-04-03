using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Services.Core.Interfaces;

namespace Tehnicharche.Services.Core
{
    public class UserManagerWrapper : IUserManagerWrapper
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserManagerWrapper(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync(string? searchTerm, int skip, int take)
        {
            var query = userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.UserName!.ToLower().Contains(term) ||
                    u.Email!.ToLower().Contains(term));
            }

            return await query
                .OrderBy(u => u.UserName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? searchTerm)
        {
            var query = userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.UserName!.ToLower().Contains(term) ||
                    u.Email!.ToLower().Contains(term));
            }

            return await query.CountAsync();
        }

        public Task<int> CountBannedAsync()
            => userManager.Users.CountAsync(u => u.IsBanned);

        public Task<ApplicationUser?> FindByIdAsync(string userId)
            => userManager.FindByIdAsync(userId);

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
            => userManager.GetRolesAsync(user);

        public Task<bool> IsInRoleAsync(ApplicationUser user, string role)
            => userManager.IsInRoleAsync(user, role);

        public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
            => userManager.AddToRoleAsync(user, role);

        public Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role)
            => userManager.RemoveFromRoleAsync(user, role);

        public Task<IdentityResult> UpdateAsync(ApplicationUser user)
            => userManager.UpdateAsync(user);

        public Task<IdentityResult> UpdateSecurityStampAsync(ApplicationUser user)
            => userManager.UpdateSecurityStampAsync(user);
    }
}