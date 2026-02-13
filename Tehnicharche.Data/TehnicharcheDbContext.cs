using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Data
{
    public class TehnicharcheDbContext : IdentityDbContext<ApplicationUser>
    {
        public TehnicharcheDbContext(DbContextOptions<TehnicharcheDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;

        public virtual DbSet<Post> Posts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
