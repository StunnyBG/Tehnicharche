using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tehnicharche.Data.Models
{
    public class SavedListing
    {
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        public int ListingId { get; set; }

        [ForeignKey(nameof(ListingId))]
        public virtual Listing Listing { get; set; } = null!;
    }
}
