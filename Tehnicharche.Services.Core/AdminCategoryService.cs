using Microsoft.Extensions.Caching.Memory;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly IAdminCategoryRepository repo;
        private readonly IMemoryCache cache;
        private const string CacheKey = "Categories:All";

        public AdminCategoryService(IAdminCategoryRepository repo, IMemoryCache cache)
        {
            this.repo = repo;
            this.cache = cache;
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
            cache.Remove(CacheKey);
        }

        public async Task<EditCategoryViewModel> GetForEditAsync(int id)
        {
            var c = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Category not found.");
            return new EditCategoryViewModel { Id = c.Id, Name = c.Name };
        }

        public async Task UpdateAsync(EditCategoryViewModel model)
        {
            var c = await repo.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException("Category not found.");

            var name = model.Name.Trim();
            if (await repo.NameExistsAsync(name, excludeId: model.Id))
                throw new InvalidOperationException($"A category named \"{name}\" already exists.");

            c.Name = name;
            await repo.SaveChangesAsync();
            cache.Remove(CacheKey);
        }

        public async Task DeleteAsync(int id)
        {
            var c = await repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Category not found.");

            if (await repo.IsInUseAsync(id))
                throw new InvalidOperationException(
                    "Cannot delete — one or more listings are assigned to this category. " +
                    "Reassign or delete those listings first.");

            await repo.DeleteAsync(c);
            await repo.SaveChangesAsync();
            cache.Remove(CacheKey);
        }
    }
}
