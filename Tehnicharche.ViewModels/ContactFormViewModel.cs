using System.ComponentModel.DataAnnotations;
using static Tehnicharche.GCommon.ValidationConstants.ContactForm;


namespace Tehnicharche.ViewModels
{
    public class ContactFormViewModel
    {
        [Required]
        [Display(Name = "Your Name")]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;

        [Phone]
        [Display(Name = "Phone Number")]
        [MaxLength(PhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Subject")]
        [MaxLength(SubjectMaxLength)]
        public string Subject { get; set; } = null!;

        [Required]
        [Display(Name = "Message")]
        [MinLength(MessageMinLength)]
        [MaxLength(MessageMaxLength)]
        public string Message { get; set; } = null!;
    }
}
