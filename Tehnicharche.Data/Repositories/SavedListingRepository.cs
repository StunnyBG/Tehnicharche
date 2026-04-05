using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class SavedListingRepository : ISavedListingRepository
    {
        private readonly TehnicharcheDbContext context;

        public SavedListingRepository(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> IsSavedAsync(string userId, int listingId)
            => await context.SavedListings
                .AnyAsync(sl => sl.UserId == userId && sl.ListingId == listingId);

        public async Task SaveAsync(string userId, int listingId)
        {
            var alreadySaved = await IsSavedAsync(userId, listingId);
            if (!alreadySaved)
            {
                await context.SavedListings.AddAsync(
                    new SavedListing { UserId = userId, ListingId = listingId });
                await context.SaveChangesAsync();
            }
        }

        public async Task UnsaveAsync(string userId, int listingId)
        {
            var entry = await context.SavedListings
                .FirstOrDefaultAsync(sl => sl.UserId == userId && sl.ListingId == listingId);

            if (entry != null)
            {
                context.SavedListings.Remove(entry);
                await context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetSavedByUserPagedAsync(
            string userId, int page, int pageSize, string? searchTerm)
        {
            var savedIds = context.SavedListings
                .Where(sl => sl.UserId == userId)
                .Select(sl => sl.ListingId);

            var query = context.Listings
                .AsNoTracking()
                .Where(l => savedIds.Contains(l.Id))
                .Include(l => l.Category)
                .Include(l => l.Region)
                .Include(l => l.City)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    EF.Functions.Like(l.Title.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(l.Category.Name.ToLower(), $"%{term}%"));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task DeleteByListingIdAsync(int listingId)
        {
            var entries = context.SavedListings
                .Where(sl => sl.ListingId == listingId);

            context.SavedListings.RemoveRange(entries);
            await context.SaveChangesAsync();
        }
    }
}