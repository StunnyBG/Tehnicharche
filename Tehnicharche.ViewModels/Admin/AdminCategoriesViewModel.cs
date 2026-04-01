namespace Tehnicharche.ViewModels.Admin
{
    public class AdminCategoriesViewModel
    {
        public IEnumerable<AdminCategoryRowViewModel> Categories { get; set; }
            = new List<AdminCategoryRowViewModel>();
    }
}
