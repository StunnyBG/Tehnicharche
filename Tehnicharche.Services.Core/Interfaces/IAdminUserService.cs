using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IAdminUserService
    {
        Task<AdminUsersViewModel> GetUsersAsync(int page = 1, string? searchTerm = null);

        Task<AdminDashboardViewModel> GetDashboardStatsAsync();

        Task ToggleRoleAsync(string userId, string role, string currentAdminId);

        Task BanAsync(string userId);

        Task UnbanAsync(string userId);
    }
}
