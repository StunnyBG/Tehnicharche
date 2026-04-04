using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class ListingsController : AdminBaseController
    {
        private readonly IAdminListingService listingService;
        private readonly ILogger<ListingsController> logger;

        public ListingsController(
            IAdminListingService listingService,
            ILogger<ListingsController> logger)
        {
            this.listingService = listingService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string filter = "all", string? search = null, int page = DefaultPage)
        {
            var model = await listingService.GetListingsAsync(filter, search, page);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id, string filter = "all")
        {
            try
            {
                await listingService.SoftDeleteAsync(id);
                TempData["AdminSuccess"] = $"Listing #{id} moved to trash.";
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to soft-delete listing {ListingId}.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id, string filter = "all")
        {
            try
            {
                await listingService.RestoreAsync(id);
                TempData["AdminSuccess"] = $"Listing #{id} restored.";
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to restore listing {ListingId}.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> HardDelete(int id, string filter = "all")
        {
            try
            {
                await listingService.HardDeleteAsync(id);
                TempData["AdminSuccess"] = $"Listing #{id} permanently deleted.";
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to hard-delete listing {ListingId}.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { filter });
        }
    }
}
