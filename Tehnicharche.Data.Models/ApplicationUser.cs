
using Microsoft.AspNetCore.Identity;

namespace Tehnicharche.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Listing> Listings { get; set; } = new HashSet<Listing>();
    }
}
