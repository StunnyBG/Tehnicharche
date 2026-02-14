
namespace Tehnicharche.ViewModels
{
    public class ListingIndexViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Price { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public string RegionName { get; set; } = null!;

        public string? CityName { get; set; }

        public string? ImageUrl { get; set; }
    }
}
