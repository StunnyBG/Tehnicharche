// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Threading;
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
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager   = userManager;
            _userStore     = userStore;
            _logger        = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }
        public string ReturnUrl           { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Username")]
            [StringLength(50, MinimumLength = 3)]
            public string Username { get; set; }
        }

        // Called when the user clicks "Sign in with X"
        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback",
                                       values: new { returnUrl });
            var properties  = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // OAuth provider redirects back here
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Try to sign in with the existing external login association
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {Provider}.",
                    info.Principal.Identity?.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl });
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            // No existing association — ask the user to confirm details for a new account
            ReturnUrl            = returnUrl;
            ProviderDisplayName  = info.ProviderDisplayName;

            var email    = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var userName = info.Principal.FindFirstValue(ClaimTypes.Name)
                           ?? email.Split('@')[0];

            Input = new InputModel
            {
                Email    = email,
                Username = SanitiseUsername(userName)
            };

            return Page();
        }

        // Called when the user submits the confirmation form
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            ProviderDisplayName = info.ProviderDisplayName;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check username uniqueness
            if (await _userManager.FindByNameAsync(Input.Username) != null)
            {
                ModelState.AddModelError(nameof(Input.Username), "That username is already taken.");
                return Page();
            }

            var user = new ApplicationUser();
            await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);

            var emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            // Mark email as confirmed (it was verified by the external provider)
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            createResult = await _userManager.AddLoginAsync(user, info);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            _logger.LogInformation("User created account using {Provider}.", info.LoginProvider);
            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }

        // Strip characters that are invalid in usernames
        private static string SanitiseUsername(string raw)
        {
            var sb = new StringBuilder();
            foreach (var ch in raw)
            {
                if (char.IsLetterOrDigit(ch) || ch == '.' || ch == '_' || ch == '-')
                    sb.Append(ch);
            }
            return sb.ToString().TrimEnd('.', '_', '-');
        }
    }
}
