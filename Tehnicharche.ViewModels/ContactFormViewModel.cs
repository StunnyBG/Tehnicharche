using System.ComponentModel.DataAnnotations;
using static Tehnicharche.GCommon.ValidationConstants.ContactForm;


namespace Tehnicharche.ViewModels
{
    public class ContactFormViewModel
    {
        [Required]
        [Display(Name = NameDisplayName)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = EmailDisplayName)]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;

        [Phone]
        [Display(Name = PhoneNumberDisplayName)]
        [MaxLength(PhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = SubjectDisplayName)]
        [MaxLength(SubjectMaxLength)]
        public string Subject { get; set; } = null!;

        [Required]
        [Display(Name = MessageDisplayName)]
        [MinLength(MessageMinLength)]
        [MaxLength(MessageMaxLength)]
        public string Message { get; set; } = null!;
    }
}
