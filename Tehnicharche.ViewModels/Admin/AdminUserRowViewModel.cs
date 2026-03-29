namespace Tehnicharche.ViewModels.Admin
{
    public class AdminUserRowViewModel
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public int ListingCount { get; set; }
        public bool IsBanned { get; set; }

        public IEnumerable<string> Roles { get; set; } 
            = new List<string>();
    }
}
