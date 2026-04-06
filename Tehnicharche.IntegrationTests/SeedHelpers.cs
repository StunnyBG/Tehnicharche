
using Tehnicharche.Data.Models;

namespace Tehnicharche.Tests
{
    public static class SeedHelpers
    {
        public static ApplicationUser MakeUser(string id = "user-1", string userName = "testuser", bool isBanned = false)
            => new ApplicationUser
            {
                Id = id,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = $"{userName}@test.com",
                NormalizedEmail = $"{userName}@test.com".ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString(),
                IsBanned = isBanned
            };

        public static Category MakeCategory(int id = 1, string name = "Electronics Repair")
            => new Category { Id = id, Name = name };

        public static Region MakeRegion(int id = 1, string name = "Sofia City")
            => new Region { Id = id, Name = name };

        public static City MakeCity(int id = 1, string name = "Sofia", int regionId = 1)
            => new City { Id = id, Name = name, RegionId = regionId };

        public static Listing MakeListing(
            int id,
            string creatorId,
            int categoryId = 1,
            int regionId = 1,
            int? cityId = null,
            decimal price = 50m,
            bool isDeleted = false,
            string title = "Test Listing")
            => new Listing
            {
                Id = id,
                Title = title,
                Description = "Test description",
                Price = price,
                CategoryId = categoryId,
                RegionId = regionId,
                CityId = cityId,
                CreatorId = creatorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = isDeleted
            };

        public static ContactMessage MakeContactMessage(int id = 1, bool isRead = false, string subject = "Test Subject")
            => new ContactMessage
            {
                Id = id,
                Name = "Test User",
                Email = "test@test.com",
                Subject = subject,
                Message = "This is a test message body.",
                IsRead = isRead,
                SentAt = DateTime.UtcNow
            };
    }
}