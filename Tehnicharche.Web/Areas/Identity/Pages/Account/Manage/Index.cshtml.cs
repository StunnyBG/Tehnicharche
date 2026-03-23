#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser>  _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser>  userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager   = userManager;
            _signInManager = signInManager;
        }

        // ── Display-only properties ──────────────────────────────────────────
        public string CurrentUsername { get; set; }
        public string CurrentEmail    { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        // ── Input models — one per form handler ──────────────────────────────

        [BindProperty]
        public PhoneInputModel PhoneInput { get; set; }

        [BindProperty]
        public UsernameInputModel UsernameInput { get; set; }

        [BindProperty]
        public EmailInputModel EmailInput { get; set; }

        public class PhoneInputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            [MaxLength(20)]
            public string PhoneNumber { get; set; }
        }

        public class UsernameInputModel
        {
            [Required]
            [Display(Name = "New username")]
            [StringLength(50, MinimumLength = 3,
                ErrorMessage = "Username must be between 3 and 50 characters.")]
            [RegularExpression(@"^[a-zA-Z0-9._\-]+$",
                ErrorMessage = "Username may only contain letters, digits, '.', '_', and '-'.")]
            public string NewUsername { get; set; }
        }

        public class EmailInputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email address")]
            [MaxLength(256)]
            public string NewEmail { get; set; }
        }

        // ── Load helper ──────────────────────────────────────────────────────

        private async Task LoadAsync(ApplicationUser user)
        {
            CurrentUsername       = user.UserName;
            CurrentEmail          = user.Email;
            PhoneInput            = new PhoneInputModel    { PhoneNumber = user.PhoneNumber };
            UsernameInput         = new UsernameInputModel { NewUsername  = user.UserName   };
            EmailInput            = new EmailInputModel    { NewEmail     = user.Email      };
        }

        // ── GET ──────────────────────────────────────────────────────────────

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            await LoadAsync(user);
            return Page();
        }

        // ── POST: phone ──────────────────────────────────────────────────────

        public async Task<IActionResult> OnPostPhoneAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Clear binding errors from the other two forms, then validate only PhoneInput
            ModelState.Clear();
            TryValidateModel(PhoneInput, nameof(PhoneInput));

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (PhoneInput.PhoneNumber != user.PhoneNumber)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, PhoneInput.PhoneNumber);
                if (!result.Succeeded)
                {
                    StatusMessage = "Error: Failed to update phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Phone number updated.";
            return RedirectToPage();
        }

        // ── POST: username ───────────────────────────────────────────────────

        public async Task<IActionResult> OnPostUsernameAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Clear binding errors from the other two forms, then validate only UsernameInput
            ModelState.Clear();
            TryValidateModel(UsernameInput, nameof(UsernameInput));

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var newUsername = UsernameInput.NewUsername.Trim();

            if (string.Equals(newUsername, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                StatusMessage = "That is already your username.";
                return RedirectToPage();
            }

            var existing = await _userManager.FindByNameAsync(newUsername);
            if (existing != null)
            {
                ModelState.AddModelError(
                    $"{nameof(UsernameInput)}.{nameof(UsernameInput.NewUsername)}",
                    "That username is already taken.");
                await LoadAsync(user);
                return Page();
            }

            var result = await _userManager.SetUserNameAsync(user, newUsername);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Username updated.";
            return RedirectToPage();
        }

        // ── POST: email ──────────────────────────────────────────────────────

        public async Task<IActionResult> OnPostEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Clear binding errors from the other two forms, then validate only EmailInput
            ModelState.Clear();
            TryValidateModel(EmailInput, nameof(EmailInput));

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var newEmail = EmailInput.NewEmail.Trim().ToLowerInvariant();

            if (string.Equals(newEmail, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                StatusMessage = "That is already your email address.";
                return RedirectToPage();
            }

            var existing = await _userManager.FindByEmailAsync(newEmail);
            if (existing != null)
            {
                ModelState.AddModelError(
                    $"{nameof(EmailInput)}.{nameof(EmailInput.NewEmail)}",
                    "That email address is already in use.");
                await LoadAsync(user);
                return Page();
            }

            var setEmailResult = await _userManager.SetEmailAsync(user, newEmail);
            if (!setEmailResult.Succeeded)
            {
                foreach (var e in setEmailResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                await LoadAsync(user);
                return Page();
            }

            // No email confirmation flow configured — confirm immediately
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Email address updated.";
            return RedirectToPage();
        }
    }
}
