namespace Tehnicharche.ViewModels.Admin
{
    public class AdminCityRowViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int RegionId { get; set; }

        public string RegionName { get; set; } = null!;

        public int ListingCount { get; set; }
    }
}
