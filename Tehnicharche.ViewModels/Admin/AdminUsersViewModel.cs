using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.ViewModels.Admin
{
    public class AdminUsersViewModel
    {
        public int TotalCount { get; set; }

        public int BannedCount { get; set; }

        public int Page { get; set; } = DefaultPage;

        public int TotalPages { get; set; }

        public string? SearchTerm { get; set; }

        public IEnumerable<AdminUserRowViewModel> Users { get; set; }
            = new List<AdminUserRowViewModel>();
    }
}
