namespace Tehnicharche.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalActiveListings { get; set; }
        
        public int TotalDeletedListings { get; set; }
        
        public int TotalUsers { get; set; }
        
        public int TotalMessages { get; set; }
        
        public int UnreadMessages { get; set; }
        
        public int TotalCategories { get; set; }

        public IEnumerable<AdminListingRowViewModel> RecentListings { get; set; }
            = new List<AdminListingRowViewModel>();
        
        public IEnumerable<AdminMessageRowViewModel> RecentMessages { get; set; } 
            = new List<AdminMessageRowViewModel>();
    }
}
