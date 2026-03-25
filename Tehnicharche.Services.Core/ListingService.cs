using Microsoft.Extensions.Caching.Memory;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class ListingService : IListingService
    {
        private readonly IListingRepository listingRepository;
        private readonly IGenericRepository<Category> categoryRepository;
        private readonly IGenericRepository<Region> regionRepository;
        private readonly IGenericRepository<City> cityRepository;
        private readonly IMemoryCache cache;

        private const string CategoriesCacheKey = "Categories:All";
        private const string RegionsCacheKey = "Regions:All";
        private const string CitiesCacheKey = "Cities:All";

        public ListingService(
            IListingRepository listingRepository,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Region> regionRepository,
            IGenericRepository<City> cityRepository,
            IMemoryCache cache)
        {
            this.listingRepository = listingRepository;
            this.categoryRepository = categoryRepository;
            this.regionRepository = regionRepository;
            this.cityRepository = cityRepository;
            this.cache = cache;
        }


        public async Task<ListingIndexQueryModel> GetIndexListingsAsync(ListingIndexQueryModel query)
        {
            query.Page = query.Page <= 0 ? DefaultPage : query.Page;

            var (items, total) = await listingRepository.GetFilteredPagedAsync(
                query.Page, 
                IndexPageSize,
                query.CategoryId, 
                query.RegionId, 
                query.CityId,
                query.MinPrice, 
                query.MaxPrice,
                query.SearchTerm);

            query.TotalListings = total;

            query.Listings = items.Select(l => new ListingIndexViewModel
            {
                Id = l.Id,
                Title = l.Title,
                Price = l.Price.ToString(),
                CategoryName = l.Category.Name,
                RegionName = l.Region.Name,
                CityName = l.City?.Name,
                ImageUrl = l.ImageUrl
            }).ToList();

            query.Categories = await GetAllCategoriesAsync();
            query.Regions = await GetAllRegionsAsync();
            query.Cities = await GetAllCitiesAsync();

            return query;
        }


        public async Task<MyListingsQueryModel> GetMyListingsAsync(MyListingsQueryModel query, string creatorId)
        {
            query.Page = query.Page <= 0 ? DefaultPage : query.Page;

            var (items, total) = await listingRepository.GetByCreatorPagedAsync(
                creatorId,
                query.Page,
                MyListingsPageSize,
                query.SearchTerm);

            query.TotalListings = total;

            query.Listings = items.Select(l => new ListingIndexViewModel
            {
                Id = l.Id,
                Title = l.Title,
                Price = l.Price.ToString(),
                CategoryName = l.Category.Name,
                RegionName = l.Region.Name,
                CityName = l.City?.Name,
                ImageUrl = l.ImageUrl
            }).ToList();

            return query;
        }


        public async Task<ListingDetailsViewModel> GetListingDetailsByIdAsync(int id)
        {
            var listing = await listingRepository.GetByIdWithDetailsAsync(id)
                ?? throw new InvalidOperationException("Listing not found.");

            return new ListingDetailsViewModel
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price.ToString(),
                CategoryName = listing.Category.Name,
                RegionName = listing.Region.Name,
                CityName = listing.City?.Name ?? string.Empty,
                ImageUrl = listing.ImageUrl,
                CreatorName = listing.Creator.UserName!,
                CreatorEmail = listing.Creator.Email!,
                CreatorPhoneNumber = listing.Creator.PhoneNumber,
                CreatorId = listing.CreatorId,
                CreatedAt = listing.CreatedAt.ToString(DateFormat),
                UpdatedAt = listing.UpdatedAt.ToString(DateFormat)
            };
        }


        public async Task<ListingCreateViewModel> GetListingCreateViewModelAsync()
        {
            return new ListingCreateViewModel
            {
                Categories = await GetAllCategoriesAsync(),
                Regions = await GetAllRegionsAsync(),
                Cities = await GetAllCitiesAsync()
            };
        }

        public async Task AddListingAsync(ListingCreateViewModel model, string creatorId)
        {
            await ValidateCategoryRegionCityAsync(model.CategoryId, model.RegionId, model.CityId);

            var listing = new Listing
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId!.Value,
                RegionId = model.RegionId!.Value,
                CityId = model.CityId,
                ImageUrl = model.ImageUrl,
                CreatorId = creatorId
            };

            await listingRepository.AddAsync(listing);
            await listingRepository.SaveChangesAsync();
        }


        public async Task<ListingEditViewModel> GetListingEditAsync(int id, string userId)
        {
            var listing = await listingRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Listing not found.");

            if (listing.CreatorId != userId)
                throw new UnauthorizedAccessException("You are not authorized to make this action.");

            return new ListingEditViewModel
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                CategoryId = listing.CategoryId,
                RegionId = listing.RegionId,
                CityId = listing.CityId,
                ImageUrl = listing.ImageUrl,
                Categories = await GetAllCategoriesAsync(),
                Regions = await GetAllRegionsAsync(),
                Cities = await GetAllCitiesAsync()
            };
        }

        public async Task EditListingAsync(ListingEditViewModel model, string userId)
        {
            var listing = await listingRepository.GetByIdTrackedAsync(model.Id)
                ?? throw new InvalidOperationException("Listing not found.");

            if (listing.CreatorId != userId)
                throw new UnauthorizedAccessException("You are not authorized to make this action.");

            await ValidateCategoryRegionCityAsync(model.CategoryId, model.RegionId, model.CityId);

            listing.Title = model.Title;
            listing.Description = model.Description;
            listing.Price = model.Price;
            listing.CategoryId = model.CategoryId!.Value;
            listing.RegionId = model.RegionId!.Value;
            listing.CityId = model.CityId;
            listing.ImageUrl = model.ImageUrl;
            listing.UpdatedAt = DateTime.UtcNow;

            await listingRepository.SaveChangesAsync();
        }


        public async Task<ListingDeleteViewModel> GetListingDeleteDetailsAsync(int id, string userId)
        {
            var listing = await listingRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Listing not found.");

            if (listing.CreatorId != userId)
                throw new UnauthorizedAccessException("You are not authorized to make this action.");

            return new ListingDeleteViewModel
            {
                Id = listing.Id,
                Title = listing.Title
            };
        }

        public async Task DeleteListingAsync(int id, string userId)
        {
            var listing = await listingRepository.GetByIdTrackedAsync(id)
                ?? throw new InvalidOperationException("Listing not found.");

            if (listing.CreatorId != userId)
                throw new UnauthorizedAccessException("You are not authorized to make this action.");

            await listingRepository.SoftDeleteAsync(listing);
        }


        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
        {
            if (cache.TryGetValue(CategoriesCacheKey, out IEnumerable<CategoryViewModel>? cached) && cached is not null)
                return cached;

            var categories = await categoryRepository.GetAllAsync();

            var categoryViewModels = categories
                .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name })
                .ToList();

            cache.Set(CategoriesCacheKey, categoryViewModels, TimeSpan.FromHours(6));
            return categoryViewModels;
        }

        public async Task<IEnumerable<RegionViewModel>> GetAllRegionsAsync()
        {
            if (cache.TryGetValue(RegionsCacheKey, out IEnumerable<RegionViewModel>? cached) && cached is not null)
                return cached;

            var regions = await regionRepository.GetAllAsync();
            
            var regionViewModels = regions
                .Select(r => new RegionViewModel { Id = r.Id, Name = r.Name })
                .ToList();

            cache.Set(RegionsCacheKey, regionViewModels, TimeSpan.FromHours(6));
            return regionViewModels;
        }

        public async Task<IEnumerable<CityViewModel>> GetAllCitiesAsync()
        {
            if (cache.TryGetValue(CitiesCacheKey, out IEnumerable<CityViewModel>? cached) && cached is not null)
                return cached;

            var cities = await cityRepository.GetAllAsync();

            var cityViewModels = cities
                .Select(c => new CityViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    RegionId = c.RegionId
                })
                .ToList();

            cache.Set(CitiesCacheKey, cityViewModels, TimeSpan.FromHours(6));
            return cityViewModels;
        }


        // helper method
        private async Task ValidateCategoryRegionCityAsync(int? categoryId, int? regionId, int? cityId)
        {
            if (!await categoryRepository.ExistsAsync(c => c.Id == categoryId))
                throw new InvalidOperationException("Invalid category id.");

            if (!await regionRepository.ExistsAsync(r => r.Id == regionId))
                throw new InvalidOperationException("Invalid region id.");

            if (cityId.HasValue)
            {
                var city = await cityRepository.GetByIdAsync(cityId.Value)
                    ?? throw new InvalidOperationException("Invalid city id.");

                if (city.RegionId != regionId)
                    throw new InvalidOperationException("City does not belong to the selected region.");
            }
        }
    }
}