using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly IAdminCategoryRepository repo;
        private readonly IMemoryCache cache;
        private readonly ILogger<AdminCategoryService> logger;

        public AdminCategoryService(
            IAdminCategoryRepository repo,
            IMemoryCache cache,
            ILogger<AdminCategoryService> logger)
        {
            this.repo = repo;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<AdminCategoriesViewModel> GetCategoriesAsync()
        {
            var categories = await repo.GetAllAsync();
            var counts = await repo.GetListingCountsAsync();

            return new AdminCategoriesViewModel
            {
                Categories = categories.Select(c => new AdminCategoryRowViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ListingCount = counts.GetValueOrDefault(c.Id)
                })
            };
        }

        public async Task AddAsync(string name)
        {
            name = name.Trim();

            if (await repo.NameExistsAsync(name))
                throw new InvalidOperationException($"A category named \"{name}\" already exists.");

            await repo.AddAsync(new Category { Name = name });
            await repo.SaveChangesAsync();
            cache.Remove(CategoriesCacheKey);

            logger.LogInformation("Category '{CategoryName}' added by admin.", name);
        }

        public async Task<EditCategoryViewModel> GetForEditAsync(int id)
        {
            var c = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Category {id} not found.");

            return new EditCategoryViewModel { Id = c.Id, Name = c.Name };
        }

        public async Task UpdateAsync(EditCategoryViewModel model)
        {
            var c = await repo.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException($"Category {model.Id} not found.");

            var name = model.Name.Trim();

            if (await repo.NameExistsAsync(name, excludeId: model.Id))
                throw new InvalidOperationException($"A category named \"{name}\" already exists.");

            c.Name = name;
            await repo.SaveChangesAsync();
            cache.Remove(CategoriesCacheKey);

            logger.LogInformation(
                "Category {CategoryId} renamed to '{CategoryName}' by admin.", model.Id, name);
        }

        public async Task DeleteAsync(int id)
        {
            var c = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Category {id} not found.");

            if (await repo.IsInUseAsync(id))
                throw new InvalidOperationException(
                    "Cannot delete — one or more listings are assigned to this category. " +
                    "Reassign or delete those listings first.");

            await repo.DeleteAsync(c);
            await repo.SaveChangesAsync();
            cache.Remove(CategoriesCacheKey);

            logger.LogInformation("Category {CategoryId} deleted by admin.", id);
        }
    }
}