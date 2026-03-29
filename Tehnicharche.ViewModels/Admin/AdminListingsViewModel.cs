namespace Tehnicharche.ViewModels.Admin
{
    public class AdminListingsViewModel
    {
        public string Filter { get; set; } = "all";
        public string? SearchTerm { get; set; }
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int DeletedCount { get; set; }

        public IEnumerable<AdminListingRowViewModel> Listings { get; set; } 
            = new List<AdminListingRowViewModel>();
    }
}
