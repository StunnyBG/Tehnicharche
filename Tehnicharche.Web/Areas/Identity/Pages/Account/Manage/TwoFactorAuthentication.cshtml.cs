#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TwoFactorAuthenticationModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public bool Is2faEnabled { get; set; }
        public int RecoveryCodesLeft { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            return Page();
        }
    }
}
