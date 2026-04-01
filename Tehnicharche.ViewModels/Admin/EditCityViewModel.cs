using System.ComponentModel.DataAnnotations;
using static Tehnicharche.GCommon.ValidationConstants.City;

namespace Tehnicharche.ViewModels.Admin
{
    public class EditCityViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength, ErrorMessage = NameErrorMessage)]
        [Display(Name = DisplayName)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = RegionErrorMessage)]
        [Display(Name = RegionDisplayName)]
        public int RegionId { get; set; }

        public IEnumerable<AdminRegionRowViewModel> Regions { get; set; }
            = new List<AdminRegionRowViewModel>();
    }
}
