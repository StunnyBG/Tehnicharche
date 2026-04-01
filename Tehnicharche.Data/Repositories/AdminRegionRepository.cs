using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class AdminRegionRepository : IAdminRegionRepository
    {
        private readonly TehnicharcheDbContext context;

        public AdminRegionRepository(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
            => await context.Regions.AsNoTracking().OrderBy(r => r.Name).ToListAsync();

        public async Task<Region?> GetByIdAsync(int id)
            => await context.Regions.FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Dictionary<int, int>> GetListingCountsAsync()
            => await context.Listings
                .IgnoreQueryFilters()
                .GroupBy(l => l.RegionId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

        public async Task<Dictionary<int, int>> GetCityCountsAsync()
            => await context.Cities
                .GroupBy(c => c.RegionId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
            => await context.Regions
                .AnyAsync(r => r.Name.ToLower() == name.ToLower()
                               && (excludeId == null || r.Id != excludeId));

        public async Task<bool> IsInUseAsync(int id)
        {
            if (await context.Cities.AnyAsync(c => c.RegionId == id)) 
                return true;
            return await context.Listings.IgnoreQueryFilters().AnyAsync(l => l.RegionId == id);
        }

        public async Task AddAsync(Region region)
            => await context.Regions.AddAsync(region);

        public async Task DeleteAsync(Region region)
            => context.Regions.Remove(region);

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
