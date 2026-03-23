#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger        = logger;
        }

        // GET — just render the "you've been signed out" page
        public IActionResult OnGet()
        {
            // If someone navigates here directly while already signed out, send them home
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return Page();
        }

        // POST — called by the navbar logout button; signs out then redirects to home
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            returnUrl ??= Url.Action("Index", "Home", new { area = "" });
            return LocalRedirect(returnUrl);
        }
    }
}
