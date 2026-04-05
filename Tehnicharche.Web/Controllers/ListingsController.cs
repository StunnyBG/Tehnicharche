using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Data.Models;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Listing;

namespace Tehnicharche.Web.Controllers
{
    public class ListingsController : AuthorizedController
    {
        private readonly IListingService listingService;
        private readonly ISavedListingService savedListingService;
        private readonly ILogger<ListingsController> logger;

        public ListingsController(
            IListingService listingService,
            ISavedListingService savedListingService,
            UserManager<ApplicationUser> userManager,
            ILogger<ListingsController> logger)
            : base(userManager)
        {
            this.listingService = listingService;
            this.savedListingService = savedListingService;
            this.logger = logger;
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
            try
            {
                await savedListingService.ToggleSaveAsync(UserId, listingId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error toggling save for listing {ListingId}.", listingId);
            }

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
                var populated = await listingService.GetListingCreateViewModelAsync();
                model.Categories = populated.Categories;
                model.Regions = populated.Regions;
                model.Cities = populated.Cities;
                return View(model);
            }

            try
            {
                await listingService.AddListingAsync(model, UserId);
                return RedirectToAction(nameof(MyListings));
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid data while creating listing for user {UserId}.", UserId);
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while creating listing for user {UserId}.", UserId);
                ModelState.AddModelError(string.Empty, "Unexpected error while creating listing.");
            }

            var repopulated = await listingService.GetListingCreateViewModelAsync();
            model.Categories = repopulated.Categories;
            model.Regions = repopulated.Regions;
            model.Cities = repopulated.Cities;
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
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "User {UserId} attempted unauthorized edit of listing {ListingId}.", UserId, id);
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
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(
                    ex, "User {UserId} attempted unauthorized edit of listing {ListingId}.", UserId, model.Id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid data while editing listing {ListingId}.", model.Id);
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while editing listing {ListingId}.", model.Id);
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
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(
                    ex, "User {UserId} attempted unauthorized delete of listing {ListingId}.", UserId, id);
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
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(
                    ex, "User {UserId} attempted unauthorized delete of listing {ListingId}.", UserId, id);
                return Forbid();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}