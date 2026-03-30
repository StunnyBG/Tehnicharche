using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IAdminListingService
    {
        Task<AdminListingsViewModel> GetListingsAsync(string filter, string? searchTerm, int page);

        Task<IEnumerable<AdminListingRowViewModel>> GetRecentAsync(int count);

        Task SoftDeleteAsync(int id);

        Task RestoreAsync(int id);

        Task HardDeleteAsync(int id);

        Task SoftDeleteAllByUserAsync(string userId);
    }
}
