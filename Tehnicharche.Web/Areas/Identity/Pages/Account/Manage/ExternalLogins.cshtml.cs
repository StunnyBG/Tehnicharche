#nullable disable

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<ApplicationUser>  _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalLoginsModel(
            UserManager<ApplicationUser>  userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager   = userManager;
            _signInManager = signInManager;
        }

        public IList<UserLoginInfo>           CurrentLogins { get; set; }
        public IList<AuthenticationScheme>    OtherLogins   { get; set; }
        public bool                           ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins   = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                            .Where(s => CurrentLogins.All(l => s.Name != l.LoginProvider))
                            .ToList();

            // Can remove an external login only if the user has a password OR another login
            ShowRemoveButton = await _userManager.HasPasswordAsync(user) || CurrentLogins.Count > 1;

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                StatusMessage = "Error: The external login was not removed.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "The external login was removed.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties  = _signInManager.ConfigureExternalAuthenticationProperties(
                                provider, redirectUrl, _userManager.GetUserId(User));

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var info   = await _signInManager.GetExternalLoginInfoAsync(userId);
            if (info == null)
            {
                StatusMessage = "Error loading external login information.";
                return RedirectToPage();
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                StatusMessage = "Error: The external login was not added. "
                              + "A login with that provider may already be associated with another account.";
                return RedirectToPage();
            }

            // Clear the external login cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = $"{info.ProviderDisplayName} account linked successfully.";
            return RedirectToPage();
        }
    }
}
