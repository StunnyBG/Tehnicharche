namespace Tehnicharche.ViewModels.Admin
{
    public class AdminUsersViewModel
    {
        public int TotalCount { get; set; }
        public int BannedCount { get; set; }

        public IEnumerable<AdminUserRowViewModel> Users { get; set; } 
            = new List<AdminUserRowViewModel>();
    }
}
