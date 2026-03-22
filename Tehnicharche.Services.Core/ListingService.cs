
using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data;
using Tehnicharche.Data.Models;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class ListingService : IListingService
    {
        private readonly TehnicharcheDbContext context;

        public ListingService(TehnicharcheDbContext context)
        {
            this.context = context;
        }

        public async Task<ListingIndexQueryModel> GetIndexListingsAsync(ListingIndexQueryModel query)
        {
            query.Page = query.Page <= 0 ? DefaultPage : query.Page;

            var listingsQuery = context.Listings
                .AsNoTracking()
                .Include(l => l.Category)
                .Include(l => l.Region)
                .Include(l => l.City)
                .AsQueryable();

            if (query.CategoryId.HasValue)
                listingsQuery = listingsQuery.Where(l => l.CategoryId == query.CategoryId.Value);

            if (query.RegionId.HasValue)
                listingsQuery = listingsQuery.Where(l => l.RegionId == query.RegionId.Value);

            if (query.CityId.HasValue)
                listingsQuery = listingsQuery.Where(l => l.CityId == query.CityId.Value);

            if (query.MinPrice.HasValue)
                listingsQuery = listingsQuery.Where(l => l.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                listingsQuery = listingsQuery.Where(l => l.Price <= query.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.Trim().ToLower();
                listingsQuery = listingsQuery.Where(l =>
                    EF.Functions.Like(l.Title.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like((l.Description ?? "").ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(l.Category.Name.ToLower(), $"%{searchTerm}%"));
            }

            query.TotalListings = await listingsQuery.CountAsync();

            query.Listings = await listingsQuery
                .OrderBy(l => l.UpdatedAt)
                .Skip((query.Page - 1) * IndexPageSize)
                .Take(IndexPageSize)
                .Select(l => new ListingIndexViewModel
                {
                    Id = l.Id,
                    Title = l.Title,
                    Price = l.Price.ToString(),
                    CategoryName = l.Category.Name,
                    RegionName = l.Region.Name,
                    CityName = l.City.Name,
                    ImageUrl = l.ImageUrl
                })
                .ToListAsync();

            query.Categories = await GetAllCategoriesAsync();
            query.Regions = await GetAllRegionsAsync();
            query.Cities = await GetAllCitiesAsync();

            return query;
        }

        public async Task<MyListingsQueryModel> GetMyListingsAsync(MyListingsQueryModel query, string creatorId)
        {
            query.Page = query.Page <= 0 ? DefaultPage : query.Page;

            var listingsQuery = context.Listings
                .AsNoTracking()
                .AsQueryable()
                .Where(l => l.CreatorId == creatorId);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.Trim().ToLower();
                listingsQuery = listingsQuery.Where(l =>
                    EF.Functions.Like(l.Title.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like((l.Description ?? "").ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(l.Category.Name.ToLower(), $"%{searchTerm}%"));
            }

            query.TotalListings = await listingsQuery.CountAsync();

            query.Listings = await listingsQuery
                .OrderBy(l => l.UpdatedAt)
                .Skip((query.Page - 1) * MyListingsPageSize)
                .Take(MyListingsPageSize)
                .Select(l => new ListingIndexViewModel
                {
                    Id = l.Id,
                    Title = l.Title,
                    Price = l.Price.ToString(),
                    CategoryName = l.Category.Name,
                    RegionName = l.Region.Name,
                    CityName = l.City.Name,
                    ImageUrl = l.ImageUrl
                })
                .ToListAsync();

            return query;
        }


        public async Task<ListingDetailsViewModel> GetListingDetailsByIdAsync(int id)
        {
            var listing = await context.Listings
                .AsNoTracking()
                .Include(l => l.Category)
                .Include(l => l.Region)
                .Include(l => l.City)
                .Include(l => l.Creator)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found.");
            }

            ListingDetailsViewModel model = new ListingDetailsViewModel
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
                CreatorPhoneNumber = listing.Creator?.PhoneNumber,
                CreatorId = listing.CreatorId!,
                CreatedAt = listing.CreatedAt.ToString(DateFormat),
                UpdatedAt = listing.UpdatedAt.ToString(DateFormat)
            };

            return model;
        }
       
        public async Task<ListingCreateViewModel> GetListingCreateViewModelAsync()
        {
            IEnumerable<CategoryViewModel> categories = await GetAllCategoriesAsync();
            IEnumerable<RegionViewModel> regions = await GetAllRegionsAsync();
            IEnumerable<CityViewModel> cities = await GetAllCitiesAsync();

            ListingCreateViewModel model = new ListingCreateViewModel
            {
                Categories = categories,
                Regions = regions,
                Cities = cities
            };

            return model;
        }
      
        public async Task AddListingAsync(ListingCreateViewModel model, string creatorId)
        {
            bool categoryExists = await context.Categories.AnyAsync(c => c.Id == model.CategoryId);

            if (!categoryExists)
            {
                throw new InvalidOperationException("Invalid category id");
            }

            bool regionExists = await context.Regions.AnyAsync(r => r.Id == model.RegionId);
            
            if (!regionExists)
            {
                throw new InvalidOperationException("Invalid region id");
            }

            if (model.CityId.HasValue)
            {
                var city = await context.Cities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == model.CityId);
                
                if (city == null)
                {
                    throw new InvalidOperationException("Invalid city id");
                }
                if (city.RegionId != model.RegionId)
                {
                    throw new InvalidOperationException("City is not from this region");
                }
            }

            var listing = new Listing()
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

            await context.Listings.AddAsync(listing);
            await context.SaveChangesAsync();
        }
     
        public async Task<ListingEditViewModel> GetListingEditAsync(int id, string userId)
        {
            var listing = await context.Listings
                            .AsNoTracking()
                            .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found.");
            }

            if (listing.CreatorId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to make this action.");
            }

            IEnumerable<CategoryViewModel> categories = await GetAllCategoriesAsync();
            IEnumerable<RegionViewModel> regions = await GetAllRegionsAsync();
            IEnumerable<CityViewModel> cities = await GetAllCitiesAsync();

            ListingEditViewModel model = new ListingEditViewModel
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                CategoryId = listing.CategoryId,
                RegionId = listing.RegionId,
                CityId = listing.CityId,
                ImageUrl = listing.ImageUrl,
                Categories = categories,
                Regions = regions,
                Cities = cities
            };

            return model;
        }
      
        public async Task EditListingAsync(ListingEditViewModel model, string userId)
        {
            var listing = await context.Listings
                .FirstOrDefaultAsync(l => l.Id == model.Id);

            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found.");
            }

            if (listing.CreatorId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to make this action.");
            }

            bool categoryExists = await context.Categories.AnyAsync(c => c.Id == model.CategoryId);

            if (!categoryExists)
            {
                throw new InvalidOperationException("Invalid category id");
            }

            bool regionExists = await context.Regions.AnyAsync(r => r.Id == model.RegionId);

            if (!regionExists)
            {
                throw new InvalidOperationException("Invalid region id");
            }

            if (model.CityId.HasValue)
            {
                var city = await context.Cities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == model.CityId);

                if (city == null)
                {
                    throw new InvalidOperationException("Invalid city id");
                }
                if (city.RegionId != model.RegionId)
                {
                    throw new InvalidOperationException("City is not from this region");
                }
            }

            listing.Title = model.Title;
            listing.Description = model.Description;
            listing.Price = model.Price;
            listing.CategoryId = model.CategoryId!.Value;
            listing.RegionId = model.RegionId!.Value;
            listing.CityId = model.CityId;
            listing.ImageUrl = model.ImageUrl;
            listing.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }
      
        public async Task DeleteListingAsync(int id, string userId)
        {
            var listing = await context.Listings
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found.");
            }

            if (listing.CreatorId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to make this action.");
            }

            listing.IsDeleted = true;
            await context.SaveChangesAsync();
        }
      
        public async Task<ListingDeleteViewModel> GetListingDeleteDetailsAsync(int id, string userId)
        {
            var listing = await context.Listings
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found.");
            }

            if (listing.CreatorId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to make this action.");
            }

            ListingDeleteViewModel model = new ListingDeleteViewModel
            {
                Id = listing.Id,
                Title = listing.Title
            };

            return model;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
        {
            return await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CityViewModel>> GetAllCitiesAsync()
        {
            return await context.Cities
                .AsNoTracking()
                .Select(c => new CityViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    RegionId = c.RegionId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RegionViewModel>> GetAllRegionsAsync()
        {
            return await context.Regions
                .AsNoTracking()
                .Select(r => new RegionViewModel
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();
        }
    }
}
