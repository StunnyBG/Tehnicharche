using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class RegionsController : AdminBaseController
    {
        private readonly IAdminRegionService regionService;

        public RegionsController(IAdminRegionService regionService)
        {
            this.regionService = regionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await regionService.GetRegionsAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["AdminError"] = "Region name cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await regionService.AddAsync(name);
                TempData["AdminSuccess"] = $"Region \"{name.Trim()}\" added.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await regionService.GetForEditAsync(id);
                return View(model);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRegionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await regionService.UpdateAsync(model);
                TempData["AdminSuccess"] = "Region updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await regionService.DeleteAsync(id);
                TempData["AdminSuccess"] = "Region deleted.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
