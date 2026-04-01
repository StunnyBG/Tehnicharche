namespace Tehnicharche.ViewModels.Admin
{
    public class AdminRegionsViewModel
    {
        public IEnumerable<AdminRegionRowViewModel> Regions { get; set; }
            = new List<AdminRegionRowViewModel>();
    }
}
