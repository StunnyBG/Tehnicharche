using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class AdminCityService : IAdminCityService
    {
        private readonly IAdminCityRepository cityRepo;
        private readonly IAdminRegionRepository regionRepo;
        private readonly IMemoryCache cache;
        private readonly ILogger<AdminCityService> logger;

        public AdminCityService(
            IAdminCityRepository cityRepo,
            IAdminRegionRepository regionRepo,
            IMemoryCache cache,
            ILogger<AdminCityService> logger)
        {
            this.cityRepo = cityRepo;
            this.regionRepo = regionRepo;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<AdminCitiesViewModel> GetCitiesAsync()
        {
            var cities = await cityRepo.GetAllAsync();
            var regions = (await regionRepo.GetAllAsync()).ToList();
            var listingCounts = await cityRepo.GetListingCountsAsync();

            return new AdminCitiesViewModel
            {
                Cities = cities.Select(c => new AdminCityRowViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    RegionId = c.RegionId,
                    RegionName = regions.FirstOrDefault(r => r.Id == c.RegionId)?.Name ?? "—",
                    ListingCount = listingCounts.GetValueOrDefault(c.Id)
                }),
                AvailableRegions = regions.Select(r => new AdminRegionRowViewModel
                {
                    Id = r.Id,
                    Name = r.Name
                })
            };
        }

        public async Task AddAsync(string name, int regionId)
        {
            name = name.Trim();

            if (await cityRepo.NameExistsInRegionAsync(name, regionId))
                throw new InvalidOperationException(
                    $"A city named \"{name}\" already exists in this region.");

            await cityRepo.AddAsync(new City { Name = name, RegionId = regionId });
            await cityRepo.SaveChangesAsync();
            cache.Remove(CitiesCacheKey);

            logger.LogInformation(
                "City '{CityName}' added to region {RegionId} by admin.", name, regionId);
        }

        public async Task<EditCityViewModel> GetForEditAsync(int id)
        {
            var city = await cityRepo.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"City {id} not found.");

            var regions = (await regionRepo.GetAllAsync())
                .Select(r => new AdminRegionRowViewModel { Id = r.Id, Name = r.Name })
                .ToList();

            return new EditCityViewModel
            {
                Id = city.Id,
                Name = city.Name,
                RegionId = city.RegionId,
                Regions = regions
            };
        }

        public async Task UpdateAsync(EditCityViewModel model)
        {
            var city = await cityRepo.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException($"City {model.Id} not found.");

            var name = model.Name.Trim();

            if (await cityRepo.NameExistsInRegionAsync(name, model.RegionId, excludeId: model.Id))
                throw new InvalidOperationException(
                    $"A city named \"{name}\" already exists in this region.");

            city.Name = name;
            city.RegionId = model.RegionId;
            await cityRepo.SaveChangesAsync();
            cache.Remove(CitiesCacheKey);

            logger.LogInformation(
                "City {CityId} updated to '{CityName}' in region {RegionId} by admin.",
                model.Id, name, model.RegionId);
        }

        public async Task DeleteAsync(int id)
        {
            var city = await cityRepo.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"City {id} not found.");

            if (await cityRepo.IsInUseAsync(id))
                throw new InvalidOperationException(
                    "Cannot delete — one or more listings are assigned to this city. " +
                    "Reassign or delete those listings first.");

            await cityRepo.DeleteAsync(city);
            await cityRepo.SaveChangesAsync();
            cache.Remove(CitiesCacheKey);

            logger.LogInformation("City {CityId} deleted by admin.", id);
        }
    }
}