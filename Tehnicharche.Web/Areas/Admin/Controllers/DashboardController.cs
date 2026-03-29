using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly IAdminUserService userService;

        public DashboardController(IAdminUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await userService.GetDashboardStatsAsync();
            return View(model);
        }
    }
}
