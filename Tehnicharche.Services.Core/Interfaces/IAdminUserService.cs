using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IAdminUserService
    {
        Task<AdminUsersViewModel> GetUsersAsync();

        Task<AdminDashboardViewModel> GetDashboardStatsAsync();

        Task ToggleRoleAsync(string userId, string role, string currentAdminId);

        Task BanAsync(string userId);

        Task UnbanAsync(string userId);
    }
}
