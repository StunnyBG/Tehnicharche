using Tehnicharche.ViewModels;

namespace Tehnicharche.Services.Core.Interfaces
{
    public interface IContactService
    {
        Task SubmitAsync(ContactFormViewModel model);
    }
}