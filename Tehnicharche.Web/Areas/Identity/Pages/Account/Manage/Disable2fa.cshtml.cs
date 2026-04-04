#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public Disable2faModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                StatusMessage = "Error: Unexpected error disabling 2FA.";
                return RedirectToPage("./TwoFactorAuthentication");
            }

            // Reset the authenticator key so it can't be re-used
            await _userManager.ResetAuthenticatorKeyAsync(user);

            StatusMessage = "Two-factor authentication has been disabled. "
                          + "You can re-enable it at any time.";
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}
