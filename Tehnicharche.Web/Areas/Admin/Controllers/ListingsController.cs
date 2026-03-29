using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class ListingsController : AdminBaseController
    {
        private readonly IAdminListingService listingService;

        public ListingsController(IAdminListingService listingService)
        {
            this.listingService = listingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filter = "all", string? search = null)
        {
            var model = await listingService.GetListingsAsync(filter, search);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id, string filter = "all")
        {
            await listingService.SoftDeleteAsync(id);
            TempData["AdminSuccess"] = $"Listing #{id} moved to trash.";
            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id, string filter = "all")
        {
            await listingService.RestoreAsync(id);
            TempData["AdminSuccess"] = $"Listing #{id} restored.";
            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> HardDelete(int id, string filter = "all")
        {
            await listingService.HardDeleteAsync(id);
            TempData["AdminSuccess"] = $"Listing #{id} permanently deleted.";
            return RedirectToAction(nameof(Index), new { filter });
        }
    }
}
