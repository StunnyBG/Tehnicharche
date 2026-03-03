
using System.ComponentModel.DataAnnotations;
using Tehnicharche.GCommon;
using static Tehnicharche.GCommon.ValidationConstants.Listing;

namespace Tehnicharche.ViewModels
{
    public class ListingEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [Price(PriceMinValue, PriceMaxValue)]
        public decimal Price { get; set; } 

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        public int? RegionId { get; set; }

        public int? CityId { get; set; }

        [Url]
        [MaxLength(ImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        public IEnumerable<CategoryViewModel>? Categories { get; set; }

        public IEnumerable<RegionViewModel>? Regions { get; set; }

        public IEnumerable<CityViewModel>? Cities { get; set; }
    }
}
