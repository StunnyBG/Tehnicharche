
using Microsoft.AspNetCore.Identity;

namespace Tehnicharche.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
