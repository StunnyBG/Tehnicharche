using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Data.Models;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Web.Controllers
{
    [Authorize]
    public class ListingsController : Controller
    {
        private readonly IListingService listingService;
        private readonly UserManager<ApplicationUser> userManager;

        private string? UserId => userManager.GetUserId(User);

        public ListingsController(IListingService listingService, UserManager<ApplicationUser> userManager)
        {
            this.listingService = listingService;
            this.userManager = userManager;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await listingService.GetAllListingsAsync(UserId);
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var model = await listingService.GetListingDetailsByIdAsync(id);
                return View(model);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyListings()
        {
            if (UserId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var listings = await listingService.GetListingsByUserAsync(UserId);
            return View("MyListings", listings);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await listingService.GetListingCreateViewModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ListingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refreshed = await listingService.GetListingCreateViewModelAsync();
                model.Categories = refreshed.Categories;
                model.Regions = refreshed.Regions;
                model.Cities = refreshed.Cities;
                return View(model);
            }

            try
            {
                if (UserId == null)
                {
                    throw new InvalidOperationException("User id not found.");
                }
                await listingService.AddListingAsync(model, UserId);
                return RedirectToAction(nameof(Index));
            }
            catch (FormatException ex)
            {
                ModelState.AddModelError(nameof(model.Price), ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ModelState.AddModelError(nameof(model.Price), ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error while creating listing.");
            }

            var refreshedOnError = await listingService.GetListingCreateViewModelAsync();
            model.Categories = refreshedOnError.Categories;
            model.Regions = refreshedOnError.Regions;
            model.Cities = refreshedOnError.Cities;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (UserId == null)
                {
                    throw new InvalidOperationException("User id not found.");
                }
                var model = await listingService.GetListingEditAsync(id, UserId);
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ListingEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refreshed = await listingService.GetListingCreateViewModelAsync();
                model.Categories = refreshed.Categories;
                model.Regions = refreshed.Regions;
                model.Cities = refreshed.Cities;
                return View(model);
            }

            try
            {
                if (UserId == null)
                {
                    throw new InvalidOperationException("User id not found.");
                }
                await listingService.EditListingAsync(model, UserId);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (FormatException ex)
            {
                ModelState.AddModelError(nameof(model.Price), ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ModelState.AddModelError(nameof(model.Price), ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error while editing listing.");
            }

            var refreshedOnError = await listingService.GetListingCreateViewModelAsync();
            model.Categories = refreshedOnError.Categories;
            model.Regions = refreshedOnError.Regions;
            model.Cities = refreshedOnError.Cities;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (UserId == null)
                {
                    throw new InvalidOperationException("User id not found.");
                }
                var model = await listingService.GetListingDeleteDetailsAsync(id, UserId);
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (UserId == null)
                {
                    throw new InvalidOperationException("User id not found.");
                }
                await listingService.DeleteListingAsync(id, UserId);
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}
