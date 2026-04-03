using Microsoft.AspNetCore.Identity;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IUserManagerWrapper
    {
        Task<List<ApplicationUser>> GetUsersAsync(string? searchTerm, int skip, int take);

        Task<int> CountAsync(string? searchTerm);

        Task<int> CountBannedAsync();

        Task<ApplicationUser?> FindByIdAsync(string userId);

        Task<IList<string>> GetRolesAsync(ApplicationUser user);

        Task<bool> IsInRoleAsync(ApplicationUser user, string role);

        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);

        Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role);

        Task<IdentityResult> UpdateAsync(ApplicationUser user);

        Task<IdentityResult> UpdateSecurityStampAsync(ApplicationUser user);
    }
}
