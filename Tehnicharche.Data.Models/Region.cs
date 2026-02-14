
using System.ComponentModel.DataAnnotations;
using static Tehnicharche.GCommon.ValidationConstants.Region;

namespace Tehnicharche.Data.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<City> Cities { get; set; } = new HashSet<City>();

        public virtual ICollection<Listing> Listings { get; set; } = new HashSet<Listing>();
    }
}
