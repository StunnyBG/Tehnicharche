using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;

namespace Tehnicharche.Data.Repositories
{
    public class AdminCityRepository : IAdminCityRepository
    {
        private readonly TehnicharcheDbContext context;

        public AdminCityRepository(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
            => await context.Cities
                .AsNoTracking()
                .OrderBy(c => c.RegionId)
                .ThenBy(c => c.Name)
                .ToListAsync();

        public async Task<City?> GetByIdAsync(int id)
            => await context.Cities.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Dictionary<int, int>> GetListingCountsAsync()
            => await context.Listings
                .IgnoreQueryFilters()
                .Where(l => l.CityId != null)
                .GroupBy(l => l.CityId!.Value)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

        public async Task<bool> NameExistsInRegionAsync(string name, int regionId, int? excludeId = null)
            => await context.Cities
                .AnyAsync(c => c.Name.ToLower() == name.ToLower()
                               && c.RegionId == regionId
                               && (excludeId == null || c.Id != excludeId));

        public async Task<bool> IsInUseAsync(int id)
            => await context.Listings
                .IgnoreQueryFilters()
                .AnyAsync(l => l.CityId == id);

        public async Task AddAsync(City city)
            => await context.Cities.AddAsync(city);

        public async Task DeleteAsync(City city)
            => context.Cities.Remove(city);

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
