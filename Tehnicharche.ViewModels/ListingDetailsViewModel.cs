
namespace Tehnicharche.ViewModels
{
    public class ListingDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string Price { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public string RegionName { get; set; } = null!;

        public string? CityName { get; set; }
       
        public string? ImageUrl { get; set; }

        public string CreatorId { get; set; } = null!;
        
        public string CreatorName { get; set; } = null!;
       
        public string CreatorEmail { get; set; } = null!;
       
        public string? CreatorPhoneNumber { get; set; }

        public string CreatedAt { get; set; } = null!;

        public string UpdatedAt { get; set; } = null!;
    }
}
