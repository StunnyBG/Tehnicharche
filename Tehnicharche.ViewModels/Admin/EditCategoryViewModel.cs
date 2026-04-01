using System.ComponentModel.DataAnnotations;
using static Tehnicharche.GCommon.ValidationConstants.Category;

namespace Tehnicharche.ViewModels.Admin
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength, ErrorMessage = NameErrorMessage)]
        [Display(Name = DisplayName)]
        public string Name { get; set; } = null!;
    }
}
