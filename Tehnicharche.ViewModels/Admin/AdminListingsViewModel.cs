
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.ViewModels.Admin
{
    public class AdminListingsViewModel
    {
        public string Filter { get; set; } = "all";
        
        public string? SearchTerm { get; set; }

        public int TotalCount { get; set; }
        
        public int ActiveCount { get; set; }
        
        public int DeletedCount { get; set; }

        public int Page { get; set; } = DefaultPage;
        
        public int TotalPages { get; set; }

        public IEnumerable<AdminListingRowViewModel> Listings { get; set; }
            = new List<AdminListingRowViewModel>();
    }
}
