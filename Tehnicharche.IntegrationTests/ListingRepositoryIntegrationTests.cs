using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tehnicharche.Data;
using Tehnicharche.Data.Repositories;
using Tehnicharche.Tests.Integration.Helpers;

namespace Tehnicharche.Tests.Integration
{
    [TestFixture]
    public class ListingRepositoryIntegrationTests
    {
        private TehnicharcheDbContext context = null!;
        private ListingRepository sut = null!;

        [SetUp]
        public async Task SetUp()
        {
            context = DbContextFactory.Create();
            sut = new ListingRepository(context);

            var user = SeedHelpers.MakeUser();
            context.Users.Add(user);
            context.Categories.Add(SeedHelpers.MakeCategory(1, "Electronics"));
            context.Categories.Add(SeedHelpers.MakeCategory(2, "Appliances"));
            context.Regions.Add(SeedHelpers.MakeRegion(1, "Sofia City"));
            context.Regions.Add(SeedHelpers.MakeRegion(2, "Varna"));
            context.Cities.Add(SeedHelpers.MakeCity(1, "Sofia", 1));
            await context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown() => context.Dispose();

        // GetFilteredPagedAsync

        [Test]
        public async Task GetFilteredPagedAsync_NoFilters_ReturnsAllActiveListings()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", title: "Listing A"),
                SeedHelpers.MakeListing(2, "user-1", title: "Listing B"),
                SeedHelpers.MakeListing(3, "user-1", title: "Deleted", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetFilteredPagedAsync(1, 10, null, null, null, null, null, null);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.Count(), Is.EqualTo(2));
            Assert.That(items.Any(l => l.Title == "Deleted"), Is.False);
        }

        [Test]
        public async Task GetFilteredPagedAsync_FilterByCategory_ReturnsOnlyMatchingCategory()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", categoryId: 1),
                SeedHelpers.MakeListing(2, "user-1", categoryId: 1),
                SeedHelpers.MakeListing(3, "user-1", categoryId: 2)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetFilteredPagedAsync(1, 10, categoryId: 1, null, null, null, null, null);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(l => l.CategoryId == 1), Is.True);
        }

        [Test]
        public async Task GetFilteredPagedAsync_FilterByRegion_ReturnsOnlyMatchingRegion()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", regionId: 1),
                SeedHelpers.MakeListing(2, "user-1", regionId: 2),
                SeedHelpers.MakeListing(3, "user-1", regionId: 2)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetFilteredPagedAsync(1, 10, null, regionId: 2, null, null, null, null);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(l => l.RegionId == 2), Is.True);
        }

        [Test]
        public async Task GetFilteredPagedAsync_FilterByCity_ReturnsOnlyMatchingCity()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", cityId: 1),
                SeedHelpers.MakeListing(2, "user-1", cityId: null),
                SeedHelpers.MakeListing(3, "user-1", cityId: 1)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetFilteredPagedAsync(1, 10, null, null, cityId: 1, null, null, null);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(l => l.CityId == 1), Is.True);
        }

        [Test]
        public async Task GetFilteredPagedAsync_FilterByMinPrice_ExcludesCheaperListings()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", price: 10m),
                SeedHelpers.MakeListing(2, "user-1", price: 50m),
                SeedHelpers.MakeListing(3, "user-1", price: 100m)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetFilteredPagedAsync(1, 10, null, null, null, minPrice: 50m, null, null);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(l => l.Price >= 50m), Is.True);
        }

        [Test]
        public async Task GetFilteredPagedAsync_FilterByMaxPrice_ExcludesExpensiveListings()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", price: 10m),
                SeedHelpers.MakeListing(2, "user-1", price: 50m),
                SeedHelpers.MakeListing(3, "user-1", price: 100m)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetFilteredPagedAsync(1, 10, null, null, null, null, maxPrice: 50m, null);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(l => l.Price <= 50m), Is.True);
        }

        [Test]
        public async Task GetFilteredPagedAsync_FilterBySearchTerm_MatchesTitle()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", title: "Laptop repair service"),
                SeedHelpers.MakeListing(2, "user-1", title: "Phone screen fix"),
                SeedHelpers.MakeListing(3, "user-1", title: "LAPTOP keyboard replacement")
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetFilteredPagedAsync(1, 10, null, null, null, null, null, searchTerm: "laptop");

            Assert.That(total, Is.EqualTo(2));
        }

        [Test]
        public async Task GetFilteredPagedAsync_Pagination_ReturnsCorrectPage()
        {
            for (int i = 1; i <= 7; i++)
                context.Listings.Add(SeedHelpers.MakeListing(i, "user-1", title: $"Listing {i}"));

            await context.SaveChangesAsync();

            var (page1Items, total) = await sut.GetFilteredPagedAsync(1, 3, null, null, null, null, null, null);
            var (page2Items, _) = await sut.GetFilteredPagedAsync(2, 3, null, null, null, null, null, null);
            var (page3Items, _) = await sut.GetFilteredPagedAsync(3, 3, null, null, null, null, null, null);

            Assert.That(total, Is.EqualTo(7));
            Assert.That(page1Items.Count(), Is.EqualTo(3));
            Assert.That(page2Items.Count(), Is.EqualTo(3));
            Assert.That(page3Items.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetFilteredPagedAsync_DeletedListings_AreExcludedByQueryFilter()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1", isDeleted: true),
                SeedHelpers.MakeListing(3, "user-1", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var (_, total) = await sut.GetFilteredPagedAsync(1, 10, null, null, null, null, null, null);

            Assert.That(total, Is.EqualTo(1));
        }

        // GetByCreatorPagedAsync

        [Test]
        public async Task GetByCreatorPagedAsync_ReturnsOnlyListingsForThatCreator()
        {
            var user2 = SeedHelpers.MakeUser("user-2", "seconduser");
            context.Users.Add(user2);
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1"),
                SeedHelpers.MakeListing(3, "user-2")
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetByCreatorPagedAsync("user-1", 1, 10, null);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(l => l.CreatorId == "user-1"), Is.True);
        }

        [Test]
        public async Task GetByCreatorPagedAsync_SoftDeletedListings_AreExcluded()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetByCreatorPagedAsync("user-1", 1, 10, null);

            Assert.That(total, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByCreatorPagedAsync_SearchTerm_FiltersResultsForThatCreator()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", title: "PCB soldering service"),
                SeedHelpers.MakeListing(2, "user-1", title: "TV repair"),
                SeedHelpers.MakeListing(3, "user-1", title: "another soldering job")
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetByCreatorPagedAsync("user-1", 1, 10, "soldering");

            Assert.That(total, Is.EqualTo(2));
        }

        // GetByIdWithDetailsAsync

        [Test]
        public async Task GetByIdWithDetailsAsync_ExistingId_ReturnsListingWithNavigations()
        {
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1", categoryId: 1, regionId: 1, cityId: 1));
            await context.SaveChangesAsync();

            var result = await sut.GetByIdWithDetailsAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Category, Is.Not.Null);
            Assert.That(result.Region, Is.Not.Null);
            Assert.That(result.City, Is.Not.Null);
            Assert.That(result.Creator, Is.Not.Null);
            Assert.That(result.Category.Name, Is.EqualTo("Electronics"));
        }

        [Test]
        public async Task GetByIdWithDetailsAsync_NonExistentId_ReturnsNull()
        {
            var result = await sut.GetByIdWithDetailsAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdWithDetailsAsync_SoftDeletedListing_ReturnsNull()
        {
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1", isDeleted: true));
            await context.SaveChangesAsync();

            var result = await sut.GetByIdWithDetailsAsync(1);

            Assert.That(result, Is.Null);
        }

        // AddAsync / SaveChangesAsync

        [Test]
        public async Task AddAsync_NewListing_IsPersisted()
        {
            var listing = SeedHelpers.MakeListing(1, "user-1");

            await sut.AddAsync(listing);
            await sut.SaveChangesAsync();

            var stored = await context.Listings.FindAsync(1);
            Assert.That(stored, Is.Not.Null);
            Assert.That(stored!.Title, Is.EqualTo(listing.Title));
        }

        // SoftDeleteAsync

        [Test]
        public async Task SoftDeleteAsync_SetsIsDeletedTrue()
        {
            var listing = SeedHelpers.MakeListing(1, "user-1");
            context.Listings.Add(listing);
            await context.SaveChangesAsync();

            var tracked = await sut.GetByIdTrackedAsync(1);
            await sut.SoftDeleteAsync(tracked!);

            var stored = await context.Listings.IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == 1);
            Assert.That(stored!.IsDeleted, Is.True);
        }

        [Test]
        public async Task SoftDeleteAsync_SoftDeletedListing_IsHiddenFromNormalQueries()
        {
            var listing = SeedHelpers.MakeListing(1, "user-1");
            context.Listings.Add(listing);
            await context.SaveChangesAsync();

            var tracked = await sut.GetByIdTrackedAsync(1);
            await sut.SoftDeleteAsync(tracked!);

            var (_, total) = await sut.GetFilteredPagedAsync(1, 10, null, null, null, null, null, null);
            Assert.That(total, Is.EqualTo(0));
        }
    }
}