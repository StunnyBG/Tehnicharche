using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public abstract class AuthorizedController : Controller
    {
        protected readonly UserManager<ApplicationUser> UserManager;

        protected string UserId => UserManager.GetUserId(User)!;

        protected AuthorizedController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
    }
}
