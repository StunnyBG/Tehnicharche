#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWithRecoveryCodeModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginWithRecoveryCodeModel> _logger;

        public LoginWithRecoveryCodeModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginWithRecoveryCodeModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery code")]
            public string RecoveryCode { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return RedirectToPage("./Login");

            ReturnUrl = returnUrl ?? Url.Content("~/");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid) return Page();

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return RedirectToPage("./Login");

            var code = Input.RecoveryCode.Replace(" ", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with a recovery code.");
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            _logger.LogWarning("Invalid recovery code entered.");
            ModelState.AddModelError(string.Empty, "Invalid recovery code.");
            return Page();
        }
    }
}
