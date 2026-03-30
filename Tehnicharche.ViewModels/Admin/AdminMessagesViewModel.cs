
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.ViewModels.Admin
{
    public class AdminMessagesViewModel
    {
        public int UnreadCount { get; set; }
        
        public int TotalCount { get; set; }

        public int Page { get; set; } = DefaultPage;
        
        public int TotalPages { get; set; }

        public IEnumerable<AdminMessageRowViewModel> Messages { get; set; }
            = new List<AdminMessageRowViewModel>();
    }
}
