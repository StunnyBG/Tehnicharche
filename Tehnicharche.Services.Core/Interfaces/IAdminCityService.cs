using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IAdminCityService
    {
        Task<AdminCitiesViewModel> GetCitiesAsync();
        
        Task AddAsync(string name, int regionId);
        
        Task<EditCityViewModel> GetForEditAsync(int id);
        
        Task UpdateAsync(EditCityViewModel model);
        
        Task DeleteAsync(int id);
    }
}
