using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class ContactMessageRepository : IContactMessageRepository
    {
        private readonly TehnicharcheDbContext context;

        public ContactMessageRepository(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<(IEnumerable<ContactMessage> Items, int TotalCount)> GetAllAsync(
            string filter, int page, int pageSize)
        {
            var query = context.ContactMessages.AsQueryable();

            if (filter == "unread")
                query = query.Where(m => !m.IsRead);
            else if (filter == "read")
                query = query.Where(m => m.IsRead);

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ContactMessage?> GetByIdAsync(int id)
            => await context.ContactMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<ContactMessage?> GetByIdTrackedAsync(int id)
            => await context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<ContactMessage>> GetRecentAsync(int count)
            => await context.ContactMessages
                .AsNoTracking()
                .OrderByDescending(m => m.SentAt)
                .Take(count)
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync()
            => await context.ContactMessages.CountAsync(m => !m.IsRead);

        public async Task<int> GetTotalCountAsync()
            => await context.ContactMessages.CountAsync();

        public async Task AddAsync(ContactMessage message)
            => await context.ContactMessages.AddAsync(message);

        public async Task DeleteAsync(ContactMessage message)
        {
            context.ContactMessages.Remove(message);
            await context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
