using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class CitiesController : AdminBaseController
    {
        private readonly IAdminCityService cityService;

        public CitiesController(IAdminCityService cityService)
        {
            this.cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await cityService.GetCitiesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name, int regionId)
        {
            if (string.IsNullOrWhiteSpace(name) || regionId == 0)
            {
                TempData["AdminError"] = "City name and region are both required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await cityService.AddAsync(name, regionId);
                TempData["AdminSuccess"] = $"City \"{name.Trim()}\" added.";
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
                var model = await cityService.GetForEditAsync(id);
                return View(model);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCityViewModel model)
        {
            ModelState.Remove(nameof(EditCityViewModel.Regions));

            if (!ModelState.IsValid)
            {
                var reloaded = await cityService.GetForEditAsync(model.Id);
                model.Regions = reloaded.Regions;
                return View(model);
            }

            try
            {
                await cityService.UpdateAsync(model);
                TempData["AdminSuccess"] = "City updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                var reloaded = await cityService.GetForEditAsync(model.Id);
                model.Regions = reloaded.Regions;
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await cityService.DeleteAsync(id);
                TempData["AdminSuccess"] = "City deleted.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
