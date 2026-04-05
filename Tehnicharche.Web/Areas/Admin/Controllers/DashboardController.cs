using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly IAdminDashboardService dashboardService;

        public DashboardController(IAdminDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await dashboardService.GetDashboardStatsAsync();
            return View(model);
        }
    }
}