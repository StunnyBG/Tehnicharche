using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class AdminRegionService : IAdminRegionService
    {
        private readonly IAdminRegionRepository repo;
        private readonly IMemoryCache cache;
        private readonly ILogger<AdminRegionService> logger;

        public AdminRegionService(
            IAdminRegionRepository repo,
            IMemoryCache cache,
            ILogger<AdminRegionService> logger)
        {
            this.repo = repo;
            this.cache = cache;
            this.logger = logger;
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
            cache.Remove(RegionsCacheKey);

            logger.LogInformation("Region '{RegionName}' added by admin.", name);
        }

        public async Task<EditRegionViewModel> GetForEditAsync(int id)
        {
            var r = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Region {id} not found.");

            return new EditRegionViewModel { Id = r.Id, Name = r.Name };
        }

        public async Task UpdateAsync(EditRegionViewModel model)
        {
            var r = await repo.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException($"Region {model.Id} not found.");

            var name = model.Name.Trim();

            if (await repo.NameExistsAsync(name, excludeId: model.Id))
                throw new InvalidOperationException($"A region named \"{name}\" already exists.");

            r.Name = name;
            await repo.SaveChangesAsync();
            cache.Remove(RegionsCacheKey);

            logger.LogInformation(
                "Region {RegionId} renamed to '{RegionName}' by admin.", model.Id, name);
        }

        public async Task DeleteAsync(int id)
        {
            var r = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Region {id} not found.");

            if (await repo.IsInUseAsync(id))
                throw new InvalidOperationException(
                    "Cannot delete — this region has cities or listings associated with it. " +
                    "Delete or reassign those first.");

            await repo.DeleteAsync(r);
            await repo.SaveChangesAsync();
            cache.Remove(RegionsCacheKey);

            logger.LogInformation("Region {RegionId} deleted by admin.", id);
        }
    }
}