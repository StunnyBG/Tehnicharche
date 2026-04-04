using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly TehnicharcheDbContext context;

        public ListingRepository(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetFilteredPagedAsync(
            int page,
            int pageSize,
            int? categoryId,
            int? regionId,
            int? cityId,
            decimal? minPrice,
            decimal? maxPrice,
            string? searchTerm)
        {
            var query = context.Listings
                .AsNoTracking()
                .Include(l => l.Category)
                .Include(l => l.Region)
                .Include(l => l.City)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(l => l.CategoryId == categoryId.Value);

            if (regionId.HasValue)
                query = query.Where(l => l.RegionId == regionId.Value);

            if (cityId.HasValue)
                query = query.Where(l => l.CityId == cityId.Value);

            if (minPrice.HasValue)
                query = query.Where(l => l.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(l => l.Price <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    EF.Functions.Like(l.Title.ToLower(), $"%{term}%") ||
                    EF.Functions.Like((l.Description ?? "").ToLower(), $"%{term}%") ||
                    EF.Functions.Like(l.Category.Name.ToLower(), $"%{term}%"));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(l => l.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetByCreatorPagedAsync(
            string creatorId,
            int page,
            int pageSize,
            string? searchTerm)
        {
            var query = context.Listings
                .AsNoTracking()
                .Include(l => l.Category)
                .Include(l => l.Region)
                .Include(l => l.City)
                .Where(l => l.CreatorId == creatorId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    EF.Functions.Like(l.Title.ToLower(), $"%{term}%") ||
                    EF.Functions.Like((l.Description ?? "").ToLower(), $"%{term}%") ||
                    EF.Functions.Like(l.Category.Name.ToLower(), $"%{term}%"));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(l => l.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Listing?> GetByIdWithDetailsAsync(int id)
            => await context.Listings
                .AsNoTracking()
                .Include(l => l.Category)
                .Include(l => l.Region)
                .Include(l => l.City)
                .Include(l => l.Creator)
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<Listing?> GetByIdAsync(int id)
            => await context.Listings
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<Listing?> GetByIdTrackedAsync(int id)
            => await context.Listings
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task AddAsync(Listing listing)
            => await context.Listings.AddAsync(listing);

        public async Task SoftDeleteAsync(Listing listing)
        {
            listing.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}