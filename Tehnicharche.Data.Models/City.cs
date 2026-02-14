
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Tehnicharche.GCommon.ValidationConstants.City;


namespace Tehnicharche.Data.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public int RegionId { get; set; }

        [ForeignKey(nameof(RegionId))]
        public Region Region { get; set; } = null!;

        public virtual ICollection<Listing> Listings { get; set; } = new HashSet<Listing>();
    }
}
