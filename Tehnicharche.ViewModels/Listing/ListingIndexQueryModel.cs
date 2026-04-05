using Tehnicharche.GCommon;
using static Tehnicharche.GCommon.ApplicationConstants;
using static Tehnicharche.GCommon.ValidationConstants.Listing;

namespace Tehnicharche.ViewModels.Listing
{
    [PriceRange(PriceMinValue, PriceMaxValue)]
    public class ListingIndexQueryModel
    {
        public int Page { get; set; } = DefaultPage;

        public int? CategoryId { get; set; }

        public int? RegionId { get; set; }

        public int? CityId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? SearchTerm { get; set; }

        public int TotalListings { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalListings / IndexPageSize);

        public IEnumerable<ListingIndexViewModel> Listings { get; set; }
            = new List<ListingIndexViewModel>();

        public IEnumerable<CategoryViewModel> Categories { get; set; }
            = new List<CategoryViewModel>();

        public IEnumerable<RegionViewModel> Regions { get; set; }
            = new List<RegionViewModel>();

        public IEnumerable<CityViewModel> Cities { get; set; }
            = new List<CityViewModel>();
    }
}
