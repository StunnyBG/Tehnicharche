using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Data.Models;
using Tehnicharche.Services.Core.Interfaces;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        private readonly IAdminUserService userService;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(IAdminUserService userService, UserManager<ApplicationUser> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = DefaultPage, string? search = null)
        {
            var model = await userService.GetUsersAsync(page, search);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRole(string userId, string role)
        {
            try
            {
                await userService.ToggleRoleAsync(userId, role, userManager.GetUserId(User)!);
            }
            catch (InvalidOperationException ex)
            {
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Ban(string userId)
        {
            try
            {
                await userService.BanAsync(userId);
                TempData["AdminSuccess"] = "User banned and their listings have been removed.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Unban(string userId)
        {
            try
            {
                await userService.UnbanAsync(userId);
                TempData["AdminSuccess"] = "User has been unbanned.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
