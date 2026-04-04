#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Tehnicharche.Data.Models;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly IUserEmailStore<ApplicationUser> emailStore;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.userStore = userStore;
            this.emailStore = GetEmailStore();
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; } = null!;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [Required]
            [StringLength(100,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password",
                ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var user = CreateUser();

            await userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            if (!string.IsNullOrWhiteSpace(Input.PhoneNumber))
                user.PhoneNumber = Input.PhoneNumber;

            var result = await userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                logger.LogInformation(
                    "New user '{Username}' registered with email '{Email}'.",
                    Input.Username, Input.Email);

                await userManager.AddToRoleAsync(user, UserRole);

                var userId = await userManager.GetUserIdAsync(user);
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code, returnUrl },
                    protocol: Request.Scheme);

                await emailSender.SendEmailAsync(
                    Input.Email,
                    "Confirm your email",
                    $"Please confirm your account by " +
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation",
                        new { email = Input.Email, returnUrl });
                }

                await signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class " +
                    $"and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
                throw new NotSupportedException(
                    "The default UI requires a user store with email support.");

            return (IUserEmailStore<ApplicationUser>)userStore;
        }
    }
}