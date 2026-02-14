
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Tehnicharche.GCommon.ValidationConstants.Listing;

namespace Tehnicharche.Data.Models
{
    public class Listing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Column(TypeName = PriceColumnType)]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [Required]
        public int RegionId { get; set; }

        [ForeignKey(nameof(RegionId))]
        public virtual Region Region { get; set; } = null!;

        public int? CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public virtual City? City { get; set; } = null!;


        [MaxLength(ImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        [Required]
        public string CreatorId { get; set; } = null!;

        [ForeignKey(nameof(CreatorId))]
        public virtual ApplicationUser Creator { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
