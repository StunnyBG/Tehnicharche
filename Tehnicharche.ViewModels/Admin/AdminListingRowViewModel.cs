namespace Tehnicharche.ViewModels.Admin
{
    public class AdminListingRowViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string CreatorName { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedAt { get; set; } = null!;
    }
}
