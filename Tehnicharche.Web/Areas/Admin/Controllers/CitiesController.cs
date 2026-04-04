using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class CitiesController : AdminBaseController
    {
        private readonly IAdminCityService cityService;
        private readonly ILogger<CitiesController> logger;

        public CitiesController(
            IAdminCityService cityService,
            ILogger<CitiesController> logger)
        {
            this.cityService = cityService;
            this.logger = logger;
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
                logger.LogWarning(ex, "Failed to add city '{CityName}' to region {RegionId}.", name, regionId);
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
                logger.LogWarning(ex, "Failed to update city {CityId}.", model.Id);
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
                logger.LogWarning(ex, "Failed to delete city {CityId}.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}