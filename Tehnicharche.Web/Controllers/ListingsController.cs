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

        public ListingsController(IListingService listingService, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.listingService = listingService;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ListingIndexQueryModel query)
        {
            query = await listingService.GetIndexListingsAsync(query);

            if (!ModelState.IsValid)
            {
                query.Listings = Enumerable.Empty<ListingIndexViewModel>();
            }

            return View(query);
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
        public async Task<IActionResult> MyListings([FromQuery] MyListingsQueryModel query)
        {
            query = await listingService.GetMyListingsAsync(query, UserId);

            if (!ModelState.IsValid)
            {
                query.Listings = Enumerable.Empty<ListingIndexViewModel>();
            }

            return View(query);
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
