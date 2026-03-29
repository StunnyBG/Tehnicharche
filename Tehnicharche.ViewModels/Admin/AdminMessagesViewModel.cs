namespace Tehnicharche.ViewModels.Admin
{
    public class AdminMessagesViewModel
    {
        public int UnreadCount { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<AdminMessageRowViewModel> Messages { get; set; } 
            = new List<AdminMessageRowViewModel>();
    }
}
