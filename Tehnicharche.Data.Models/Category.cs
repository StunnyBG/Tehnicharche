
using System.ComponentModel.DataAnnotations;
using static Tehnicharche.GCommon.ValidationConstants.Category;

namespace Tehnicharche.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
