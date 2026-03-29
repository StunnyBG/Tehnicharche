using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IAdminMessageService
    {
        Task<AdminMessagesViewModel> GetMessagesAsync(string filter);

        Task<AdminMessageRowViewModel> GetByIdAsync(int id);

        Task<IEnumerable<AdminMessageRowViewModel>> GetRecentAsync(int count);

        Task MarkReadAsync(int id);

        Task MarkUnreadAsync(int id);

        Task DeleteAsync(int id);

        Task<int> GetUnreadCountAsync();
    }
}
