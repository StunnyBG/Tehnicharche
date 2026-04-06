using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tehnicharche.Data;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories;
using Tehnicharche.Tests.Integration.Helpers;

namespace Tehnicharche.Tests.Integration
{
    [TestFixture]
    public class AdminListingRepositoryIntegrationTests
    {
        private TehnicharcheDbContext context = null!;
        private AdminListingRepository sut = null!;

        [SetUp]
        public async Task SetUp()
        {
            context = DbContextFactory.Create();
            sut = new AdminListingRepository(context);

            context.Users.Add(SeedHelpers.MakeUser());
            context.Categories.Add(SeedHelpers.MakeCategory(1, "Electronics"));
            context.Regions.Add(SeedHelpers.MakeRegion(1));
            await context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown() => context.Dispose();

        // GetAdminFilteredAsync

        [Test]
        public async Task GetAdminFilteredAsync_FilterAll_IncludesDeletedListings()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1", isDeleted: true),
                SeedHelpers.MakeListing(3, "user-1", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetAdminFilteredAsync("all", null, 1, 10);

            Assert.That(total, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAdminFilteredAsync_FilterActive_OnlyReturnsNonDeleted()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetAdminFilteredAsync("active", null, 1, 10);

            Assert.That(total, Is.EqualTo(1));
            Assert.That(items.First().IsDeleted, Is.False);
        }

        [Test]
        public async Task GetAdminFilteredAsync_FilterDeleted_OnlyReturnsDeleted()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1", isDeleted: true),
                SeedHelpers.MakeListing(3, "user-1", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetAdminFilteredAsync("deleted", null, 1, 10);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(l => l.IsDeleted), Is.True);
        }

        [Test]
        public async Task GetAdminFilteredAsync_SearchByTitle_IsNotCaseSensitive()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1", title: "PCB Soldering"),
                SeedHelpers.MakeListing(2, "user-1", title: "TV Repair"),
                SeedHelpers.MakeListing(3, "user-1", title: "pcb assembly")
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetAdminFilteredAsync("all", "pcb", 1, 10);

            Assert.That(total, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAdminFilteredAsync_Pagination_CorrectPageSize()
        {
            for (int i = 1; i <= 5; i++)
                context.Listings.Add(SeedHelpers.MakeListing(i, "user-1", title: $"Listing {i}"));
            await context.SaveChangesAsync();

            var (page1, total) = await sut.GetAdminFilteredAsync("all", null, 1, 2);
            var (page2, _) = await sut.GetAdminFilteredAsync("all", null, 2, 2);
            var (page3, _) = await sut.GetAdminFilteredAsync("all", null, 3, 2);

            Assert.That(total, Is.EqualTo(5));
            Assert.That(page1.Count(), Is.EqualTo(2));
            Assert.That(page2.Count(), Is.EqualTo(2));
            Assert.That(page3.Count(), Is.EqualTo(1));
        }

        // GetActiveCountAsync / GetDeletedCountAsync

        [Test]
        public async Task GetActiveCountAsync_CountsOnlyNonDeleted()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1"),
                SeedHelpers.MakeListing(3, "user-1", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var count = await sut.GetActiveCountAsync();

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetDeletedCountAsync_CountsOnlyDeleted()
        {
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1", isDeleted: true),
                SeedHelpers.MakeListing(3, "user-1", isDeleted: true)
            );
            await context.SaveChangesAsync();

            var count = await sut.GetDeletedCountAsync();

            Assert.That(count, Is.EqualTo(2));
        }

        // SoftDeleteAllByUserAsync

        [Test]
        public async Task SoftDeleteAllByUserAsync_SetsIsDeletedOnAllUserListings()
        {
            var user2 = SeedHelpers.MakeUser("user-2", "other");
            context.Users.Add(user2);
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1"),
                SeedHelpers.MakeListing(3, "user-2")
            );
            await context.SaveChangesAsync();

            await sut.SoftDeleteAllByUserAsync("user-1");

            var user1Listings = await context.Listings
                .IgnoreQueryFilters()
                .Where(l => l.CreatorId == "user-1")
                .ToListAsync();

            var user2Listings = await context.Listings
                .IgnoreQueryFilters()
                .Where(l => l.CreatorId == "user-2")
                .ToListAsync();

            Assert.That(user1Listings.All(l => l.IsDeleted), Is.True);
            Assert.That(user2Listings.All(l => !l.IsDeleted), Is.True);
        }

        [Test]
        public async Task SoftDeleteAllByUserAsync_AlreadyDeletedListings_AreNotAffectedAgain()
        {
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1", isDeleted: true));
            await context.SaveChangesAsync();

            await sut.SoftDeleteAllByUserAsync("user-1");

            var listing = await context.Listings.IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == 1);
            Assert.That(listing!.IsDeleted, Is.True);
        }

        // GetByIdDeletedAsync

        [Test]
        public async Task GetByIdDeletedAsync_SoftDeletedListing_IsFound()
        {
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1", isDeleted: true));
            await context.SaveChangesAsync();

            var result = await sut.GetByIdDeletedAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsDeleted, Is.True);
        }

        [Test]
        public async Task GetByIdDeletedAsync_ActiveListing_IsAlsoFound()
        {
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1"));
            await context.SaveChangesAsync();

            var result = await sut.GetByIdDeletedAsync(1);

            Assert.That(result, Is.Not.Null);
        }

        // HardDeleteAsync

        [Test]
        public async Task HardDeleteAsync_RemovesListingFromDatabase()
        {
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1", isDeleted: true));
            await context.SaveChangesAsync();

            var listing = await sut.GetByIdDeletedAsync(1);
            await sut.HardDeleteAsync(listing!);

            var exists = await context.Listings.IgnoreQueryFilters().AnyAsync(l => l.Id == 1);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task HardDeleteAsync_WithSavedListings_RemovesSavedEntriesFirst()
        {
            var user2 = SeedHelpers.MakeUser("user-2", "saver");
            context.Users.Add(user2);
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1"));
            await context.SaveChangesAsync();

            context.SavedListings.Add(new SavedListing
            {
                UserId = "user-2",
                ListingId = 1
            });
            await context.SaveChangesAsync();

            var listing = await sut.GetByIdDeletedAsync(1);

            Assert.DoesNotThrowAsync(() => sut.HardDeleteAsync(listing!));
        }

        // GetListingCountsByCreatorsAsync

        [Test]
        public async Task GetListingCountsByCreatorsAsync_ReturnsCorrectCountsIncludingDeleted()
        {
            var user2 = SeedHelpers.MakeUser("user-2", "second");
            context.Users.Add(user2);
            context.Listings.AddRange(
                SeedHelpers.MakeListing(1, "user-1"),
                SeedHelpers.MakeListing(2, "user-1", isDeleted: true),
                SeedHelpers.MakeListing(3, "user-2")
            );
            await context.SaveChangesAsync();

            var counts = await sut.GetListingCountsByCreatorsAsync();

            Assert.That(counts["user-1"], Is.EqualTo(2));
            Assert.That(counts["user-2"], Is.EqualTo(1));
        }
    }
}