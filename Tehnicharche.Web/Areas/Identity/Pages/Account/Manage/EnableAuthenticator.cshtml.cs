#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account.Manage
{
    public class EnableAuthenticatorModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat =
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public EnableAuthenticatorModel(
            UserManager<ApplicationUser> userManager,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be {2}–{1} digits.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification code")]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            await LoadSharedKeyAndQrCodeUriAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError(nameof(Input.Code),
                    "Verification code is invalid. Please check your authenticator app and try again.");
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            // Generate recovery codes the first time 2FA is enabled
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData["RecoveryCodes"] = recoveryCodes;

            StatusMessage = "Two-factor authentication has been enabled.";
            return RedirectToPage("./ShowRecoveryCodes");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = FormatKey(unformattedKey!);
            AuthenticatorUri = GenerateQrCodeUri(user.Email!, unformattedKey!);
        }

        private static string FormatKey(string key)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < key.Length; i++)
            {
                if (i > 0 && i % 4 == 0) sb.Append(' ');
                sb.Append(key[i]);
            }
            return sb.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey) =>
            string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Tehnicharche"),
                _urlEncoder.Encode(email),
                unformattedKey);
    }
}
