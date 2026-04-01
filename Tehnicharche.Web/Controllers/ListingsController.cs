using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Data.Models;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Web.Controllers
{
    public class ListingsController : AuthorizedController
    {
        private readonly IListingService listingService;
        private readonly ISavedListingService savedListingService;

        public ListingsController(
            IListingService listingService,
            ISavedListingService savedListingService,
            UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.listingService = listingService;
            this.savedListingService = savedListingService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ListingIndexQueryModel query)
        {
            query = await listingService.GetIndexListingsAsync(query);

            if (!ModelState.IsValid)
                query.Listings = Enumerable.Empty<ListingIndexViewModel>();

            return View(query);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var model = await listingService.GetListingDetailsByIdAsync(id);

                if (User.Identity?.IsAuthenticated == true)
                    model.IsSaved = await savedListingService.IsSavedAsync(UserId, id);

                return View(model);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyListings([FromQuery] MyListingsQueryModel query)
        {
            query = await listingService.GetMyListingsAsync(query, UserId);

            if (!ModelState.IsValid)
                query.Listings = Enumerable.Empty<ListingIndexViewModel>();

            return View(query);
        }

        [HttpGet]
        public async Task<IActionResult> SavedListings([FromQuery] SavedListingsQueryModel query)
        {
            query = await savedListingService.GetSavedListingsAsync(query, UserId);
            return View(query);
        }


        [HttpPost]
        public async Task<IActionResult> ToggleSave(int listingId, string? returnUrl = null)
        {
            await savedListingService.ToggleSaveAsync(UserId, listingId);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Details), new { id = listingId });
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
                model = await listingService.GetListingCreateViewModelAsync();
                return View(model);
            }

            try
            {
                await listingService.AddListingAsync(model, UserId);
                return RedirectToAction(nameof(MyListings));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error while creating listing.");
            }

            model = await listingService.GetListingCreateViewModelAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await listingService.GetListingEditAsync(id, UserId);
                return View(model);
            }
            catch (UnauthorizedAccessException) 
            { 
                return Forbid();
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
                model.Categories = await listingService.GetAllCategoriesAsync();
                model.Regions = await listingService.GetAllRegionsAsync();
                model.Cities = await listingService.GetAllCitiesAsync();
                return View(model);
            }

            try
            {
                await listingService.EditListingAsync(model, UserId);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (UnauthorizedAccessException) 
            {
                return Forbid();
            }
            catch (InvalidOperationException)
            { 
                return BadRequest();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error while editing listing.");
            }

            model.Categories = await listingService.GetAllCategoriesAsync();
            model.Regions = await listingService.GetAllRegionsAsync();
            model.Cities = await listingService.GetAllCitiesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var model = await listingService.GetListingDeleteDetailsAsync(id, UserId);
                return View(model);
            }
            catch (UnauthorizedAccessException) 
            {
                return Forbid();
            }
            catch (InvalidOperationException) 
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName(nameof(Delete))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await listingService.DeleteListingAsync(id, UserId);
            }
            catch (UnauthorizedAccessException) 
            {
                return Forbid();
            }
            catch (ArgumentException) 
            { 
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
