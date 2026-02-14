
using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data;
using Tehnicharche.Data.Models;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels;
using static Tehnicharche.GCommon.ValidationConstants.Listing;
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
        
        public async Task<IEnumerable<ListingIndexViewModel>> GetAllListingsAsync(string? userId)
        {
            return await context.Listings
                .Where(l => !l.IsDeleted)
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
        }
       
        public async Task<ListingIndexViewModel?> GetListingByIdAsync(int id)
        {
            var listing = await context.Listings.Where(l => !l.IsDeleted).FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found.");
            }

            ListingIndexViewModel model = new ListingIndexViewModel
            {
                Id = listing.Id,
                Title = listing.Title,
                Price = listing.Price.ToString(),
                CategoryName = listing.Category.Name,
                RegionName = listing.Region.Name,
                CityName = listing.City.Name,
                ImageUrl = listing.ImageUrl
            };

            return model;
        }
       
        public async Task<ListingDetailsViewModel> GetListingDetailsByIdAsync(int id)
        {
            var listing = await context.Listings.Where(l => !l.IsDeleted).FirstOrDefaultAsync(l => l.Id == id);

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
                CityName = listing.City.Name,
                ImageUrl = listing.ImageUrl,
                CreatorName = listing.Creator.UserName!,
                CreatedAt = listing.CreatedAt.ToString(DateFormat),
                UpdatedAt = listing.UpdatedAt?.ToString("yyyy-MM-dd") ?? listing.CreatedAt.ToString(DateFormat)
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
            if (!decimal.TryParse(model.Price, out decimal price))
            {
                throw new FormatException("Price is not a valid decimal.");
            }

            if (price < PriceMinValue || price > PriceMaxValue)
            {
                throw new ArgumentOutOfRangeException($"Price must be between {PriceMaxValue} and {PriceMaxValue}.");
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

            if (model.CityId != null)
            {
                bool cityExists = await context.Cities.AnyAsync(c => c.Id == model.CityId);
                if (!cityExists)
                {
                    throw new InvalidOperationException("Invalid city id");
                }
            }

            var listing = new Listing()
            {
                Title = model.Title,
                Description = model.Description,
                Price = price,
                CategoryId = model.CategoryId,
                RegionId = model.RegionId,
                CityId = model.CityId,
                ImageUrl = model.ImageUrl,
                CreatorId = creatorId,
                CreatedAt = DateTime.UtcNow
            };

            context.Listings.Add(listing);
            await context.SaveChangesAsync();
        }
     
        public async Task<ListingEditViewModel> GetListingEditAsync(int id, string userId)
        {
            var listing = await context.Listings.Where(l => !l.IsDeleted).FirstOrDefaultAsync(l => l.Id == id);

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
                Price = listing.Price.ToString(),
                CategoryId = listing.Category.Id,
                RegionId = listing.Region.Id,
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
            var listing = await context.Listings.Where(l => !l.IsDeleted).FirstOrDefaultAsync(l => l.Id == model.Id);

            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found.");
            }

            if (listing.CreatorId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to make this action.");
            }

            if (!decimal.TryParse(model.Price, out decimal price))
            {
                throw new FormatException("Price is not a valid decimal.");
            }

            if (price < PriceMinValue || price > PriceMaxValue)
            {
                throw new ArgumentOutOfRangeException($"Price must be between {PriceMaxValue} and {PriceMaxValue}.");
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

            if (model.CityId != null)
            {
                bool cityExists = await context.Cities.AnyAsync(c => c.Id == model.CityId);
                if (!cityExists)
                {
                    throw new InvalidOperationException("Invalid city id");
                }
            }

            listing.Title = model.Title;
            listing.Description = model.Description;
            listing.Price = price;
            listing.CategoryId = model.CategoryId;
            listing.RegionId = model.RegionId;
            listing.CityId = model.CityId;
            listing.ImageUrl = model.ImageUrl;
            listing.UpdatedAt = DateTime.UtcNow;
        }
      
        public async Task DeleteListingAsync(int id, string userId)
        {
            var listing = await context.Listings.Where(l => !l.IsDeleted).FirstOrDefaultAsync(l => l.Id == id);

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
            var listing = await context.Listings.Where(l => !l.IsDeleted).FirstOrDefaultAsync(l => l.Id == id);

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
                .Select(c => new CityViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RegionViewModel>> GetAllRegionsAsync()
        {
            return await context.Regions
                .Select(r => new RegionViewModel
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();
        }
    }
}
