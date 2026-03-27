using System.ComponentModel.DataAnnotations;
using static Tehnicharche.GCommon.ValidationConstants.ContactForm;

namespace Tehnicharche.Data.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;

        [MaxLength(PhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(SubjectMaxLength)]
        public string Subject { get; set; } = null!;

        [Required]
        [MaxLength(MessageMaxLength)]
        public string Message { get; set; } = null!;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
