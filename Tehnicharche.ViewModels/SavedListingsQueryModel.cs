using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.ViewModels
{
    public class SavedListingsQueryModel
    {
        public int Page { get; set; } = DefaultPage;

        public string? SearchTerm { get; set; }

        public int TotalListings { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalListings / MyListingsPageSize);

        public IEnumerable<ListingIndexViewModel> Listings { get; set; }
            = new List<ListingIndexViewModel>();
    }
}
