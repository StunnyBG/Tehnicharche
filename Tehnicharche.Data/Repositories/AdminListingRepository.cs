using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class AdminListingRepository : IAdminListingRepository
    {
        private readonly TehnicharcheDbContext context;

        public AdminListingRepository(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetAdminFilteredAsync(
            string filter, string? searchTerm, int page, int pageSize)
        {
            var query = context.Listings
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(l => l.Category)
                .Include(l => l.Creator)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    EF.Functions.Like(l.Title.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(l.Creator.UserName!.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(l.Category.Name.ToLower(), $"%{term}%"));
            }

            if (filter == "active")
                query = query.Where(l => !l.IsDeleted);
            else if (filter == "deleted")
                query = query.Where(l => l.IsDeleted);

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Listing>> GetRecentAdminAsync(int count)
            => await context.Listings
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(l => l.Category)
                .Include(l => l.Creator)
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();

        public async Task<int> GetActiveCountAsync()
            => await context.Listings.CountAsync();

        public async Task<int> GetDeletedCountAsync()
            => await context.Listings
                .IgnoreQueryFilters()
                .CountAsync(l => l.IsDeleted);

        public async Task<Dictionary<string, int>> GetListingCountsByCreatorsAsync()
            => await context.Listings
                .IgnoreQueryFilters()
                .GroupBy(l => l.CreatorId)
                .Select(g => new { CreatorId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CreatorId, x => x.Count);

        public async Task SoftDeleteAllByUserAsync(string userId)
        {
            var listings = await context.Listings
                .IgnoreQueryFilters()
                .Where(l => l.CreatorId == userId && !l.IsDeleted)
                .ToListAsync();

            foreach (var l in listings)
                l.IsDeleted = true;

            await context.SaveChangesAsync();
        }

        public async Task<Listing?> GetByIdDeletedAsync(int id)
            => await context.Listings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<Listing?> GetByIdTrackedAsync(int id)
            => await context.Listings
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task HardDeleteAsync(Listing listing)
        {
            // Clean up SavedListings rows that reference this listing to avoid FK violations.
            var savedEntries = context.SavedListings
                .Where(sl => sl.ListingId == listing.Id);
            context.SavedListings.RemoveRange(savedEntries);

            context.Listings.Remove(listing);
            await context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Listing listing)
        {
            listing.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
