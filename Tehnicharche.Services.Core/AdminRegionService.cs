using Microsoft.Extensions.Caching.Memory;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core
{
    public class AdminRegionService : IAdminRegionService
    {
        private readonly IAdminRegionRepository repo;
        private readonly IMemoryCache cache;
        private const string CacheKey = "Regions:All";

        public AdminRegionService(IAdminRegionRepository repo, IMemoryCache cache)
        {
            this.repo = repo;
            this.cache = cache;
        }

        public async Task<AdminRegionsViewModel> GetRegionsAsync()
        {
            var regions = await repo.GetAllAsync();
            var listingCounts = await repo.GetListingCountsAsync();
            var cityCounts = await repo.GetCityCountsAsync();

            return new AdminRegionsViewModel
            {
                Regions = regions.Select(r => new AdminRegionRowViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    CityCount = cityCounts.GetValueOrDefault(r.Id),
                    ListingCount = listingCounts.GetValueOrDefault(r.Id)
                })
            };
        }

        public async Task AddAsync(string name)
        {
            name = name.Trim();
            if (await repo.NameExistsAsync(name))
                throw new InvalidOperationException($"A region named \"{name}\" already exists.");

            await repo.AddAsync(new Region { Name = name });
            await repo.SaveChangesAsync();
            cache.Remove(CacheKey);
        }

        public async Task<EditRegionViewModel> GetForEditAsync(int id)
        {
            var r = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Region not found.");
            return new EditRegionViewModel { Id = r.Id, Name = r.Name };
        }

        public async Task UpdateAsync(EditRegionViewModel model)
        {
            var r = await repo.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException("Region not found.");

            var name = model.Name.Trim();
            if (await repo.NameExistsAsync(name, excludeId: model.Id))
                throw new InvalidOperationException($"A region named \"{name}\" already exists.");

            r.Name = name;
            await repo.SaveChangesAsync();
            cache.Remove(CacheKey);
        }

        public async Task DeleteAsync(int id)
        {
            var r = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Region not found.");

            if (await repo.IsInUseAsync(id))
                throw new InvalidOperationException(
                    "Cannot delete — this region has cities or listings associated with it. " +
                    "Delete or reassign those first.");

            await repo.DeleteAsync(r);
            await repo.SaveChangesAsync();
            cache.Remove(CacheKey);
        }
    }
}
