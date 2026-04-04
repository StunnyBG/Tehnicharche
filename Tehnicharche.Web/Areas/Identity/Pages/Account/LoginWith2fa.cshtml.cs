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
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginWith2faModel> _logger;

        public LoginWith2faModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginWith2faModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be {2}–{1} digits.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this device")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Verify the user passed through password login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("./Login");
            }

            ReturnUrl = returnUrl ?? Url.Content("~/");
            RememberMe = rememberMe;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("./Login");
            }

            var authenticatorCode = Input.TwoFactorCode
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                authenticatorCode,
                rememberMe,
                Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with 2fa.");
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            _logger.LogWarning("Invalid authenticator code for user.");
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return Page();
        }
    }
}
