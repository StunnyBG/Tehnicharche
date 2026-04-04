using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IAdminCategoryService
    {
        Task<AdminCategoriesViewModel> GetCategoriesAsync();

        Task AddAsync(string name);

        Task<EditCategoryViewModel> GetForEditAsync(int id);

        Task UpdateAsync(EditCategoryViewModel model);

        Task DeleteAsync(int id);
    }
}
