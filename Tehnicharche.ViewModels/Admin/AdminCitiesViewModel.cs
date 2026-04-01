namespace Tehnicharche.ViewModels.Admin
{
    public class AdminCitiesViewModel
    {
        public IEnumerable<AdminCityRowViewModel> Cities { get; set; }
            = new List<AdminCityRowViewModel>();

        public IEnumerable<AdminRegionRowViewModel> AvailableRegions { get; set; }
            = new List<AdminRegionRowViewModel>();
    }
}
