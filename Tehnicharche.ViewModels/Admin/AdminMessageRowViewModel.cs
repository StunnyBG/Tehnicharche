namespace Tehnicharche.ViewModels.Admin
{
    public class AdminMessageRowViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;
        
        public string Email { get; set; } = null!;
        
        public string? PhoneNumber { get; set; }
        
        public string Subject { get; set; } = null!;
        
        public string Message { get; set; } = null!;
        
        public bool IsRead { get; set; }
        
        public string SentAt { get; set; } = null!;
    }
}
