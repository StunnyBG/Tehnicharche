namespace Tehnicharche.ViewModels.Admin
{
    public class AdminRegionRowViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;
        
        public int CityCount { get; set; }
        
        public int ListingCount { get; set; }
    }
}
