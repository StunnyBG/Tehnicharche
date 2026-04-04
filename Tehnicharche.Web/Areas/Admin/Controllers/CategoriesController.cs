using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        private readonly IAdminCategoryService categoryService;
        private readonly ILogger<CategoriesController> logger;

        public CategoriesController(
            IAdminCategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            this.categoryService = categoryService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await categoryService.GetCategoriesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["AdminError"] = "Category name cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await categoryService.AddAsync(name);
                TempData["AdminSuccess"] = $"Category \"{name.Trim()}\" added.";
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to add category '{CategoryName}'.", name);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await categoryService.GetForEditAsync(id);
                return View(model);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await categoryService.UpdateAsync(model);
                TempData["AdminSuccess"] = "Category updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to update category {CategoryId}.", model.Id);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await categoryService.DeleteAsync(id);
                TempData["AdminSuccess"] = "Category deleted.";
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to delete category {CategoryId}.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}