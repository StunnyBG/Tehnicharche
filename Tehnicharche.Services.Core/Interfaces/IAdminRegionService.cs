using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IAdminRegionService
    {
        Task<AdminRegionsViewModel> GetRegionsAsync();
        
        Task AddAsync(string name);
        
        Task<EditRegionViewModel> GetForEditAsync(int id);
        
        Task UpdateAsync(EditRegionViewModel model);
        
        Task DeleteAsync(int id);
    }
}
