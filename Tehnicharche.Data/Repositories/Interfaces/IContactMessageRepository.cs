using Tehnicharche.Data.Models;

namespace Tehnicharche.Data.Repositories.Interfaces
{
    public interface IContactMessageRepository
    {
        Task<IEnumerable<ContactMessage>> GetAllAsync(string filter = "all");

        Task<ContactMessage?> GetByIdAsync(int id);

        Task<ContactMessage?> GetByIdTrackedAsync(int id);

        Task<IEnumerable<ContactMessage>> GetRecentAsync(int count);

        Task<int> GetUnreadCountAsync();

        Task<int> GetTotalCountAsync();

        Task AddAsync(ContactMessage message);

        Task DeleteAsync(ContactMessage message);

        Task SaveChangesAsync();
    }
}
