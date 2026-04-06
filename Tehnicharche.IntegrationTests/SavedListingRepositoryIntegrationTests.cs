using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tehnicharche.Data;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories;
using Tehnicharche.Tests.Integration.Helpers;

namespace Tehnicharche.Tests.Integration
{
    [TestFixture]
    public class SavedListingRepositoryIntegrationTests
    {
        private TehnicharcheDbContext context = null!;
        private SavedListingRepository sut = null!;

        [SetUp]
        public async Task SetUp()
        {
            context = DbContextFactory.Create();
            sut = new SavedListingRepository(context);

            context.Users.Add(SeedHelpers.MakeUser("user-1", "gosho"));
            context.Users.Add(SeedHelpers.MakeUser("user-2", "pesho"));
            context.Categories.Add(SeedHelpers.MakeCategory());
            context.Regions.Add(SeedHelpers.MakeRegion());
            context.Listings.Add(SeedHelpers.MakeListing(1, "user-1"));
            context.Listings.Add(SeedHelpers.MakeListing(2, "user-1"));
            await context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown() => context.Dispose();

        // IsSavedAsync

        [Test]
        public async Task IsSavedAsync_NotSaved_ReturnsFalse()
        {
            Assert.That(await sut.IsSavedAsync("user-2", 1), Is.False);
        }

        [Test]
        public async Task IsSavedAsync_AfterSaving_ReturnsTrue()
        {
            await sut.SaveAsync("user-2", 1);

            Assert.That(await sut.IsSavedAsync("user-2", 1), Is.True);
        }

        // SaveAsync

        [Test]
        public async Task SaveAsync_CreatesRowInDatabase()
        {
            await sut.SaveAsync("user-2", 1);

            var exists = await context.SavedListings.AnyAsync(sl => sl.UserId == "user-2" && sl.ListingId == 1);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task SaveAsync_CalledTwice_DoesNotCreateDuplicateRow()
        {
            await sut.SaveAsync("user-2", 1);
            await sut.SaveAsync("user-2", 1);

            var count = await context.SavedListings.CountAsync(sl => sl.UserId == "user-2" && sl.ListingId == 1);
            Assert.That(count, Is.EqualTo(1));
        }

        // UnsaveAsync

        [Test]
        public async Task UnsaveAsync_RemovesRowFromDatabase()
        {
            await sut.SaveAsync("user-2", 1);
            await sut.UnsaveAsync("user-2", 1);

            Assert.That(await sut.IsSavedAsync("user-2", 1), Is.False);
        }

        [Test]
        public async Task UnsaveAsync_WhenNotSaved_DoesNotThrow()
        {
            Assert.DoesNotThrowAsync(() => sut.UnsaveAsync("user-2", 999));
        }

        // GetSavedByUserPagedAsync

        [Test]
        public async Task GetSavedByUserPagedAsync_ReturnsOnlyListingsSavedByThatUser()
        {
            await sut.SaveAsync("user-2", 1);

            context.SavedListings.Add(new SavedListing { UserId = "user-1", ListingId = 2 });
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetSavedByUserPagedAsync("user-2", 1, 10, null);

            Assert.That(total, Is.EqualTo(1));
            Assert.That(items.Single().Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetSavedByUserPagedAsync_SearchTerm_FiltersResults()
        {
            context.Listings.Add(SeedHelpers.MakeListing(3, "user-1", title: "Soldering service"));
            await context.SaveChangesAsync();

            await sut.SaveAsync("user-2", 1);
            await sut.SaveAsync("user-2", 3);

            var (items, total) = await sut.GetSavedByUserPagedAsync("user-2", 1, 10, "soldering");

            Assert.That(total, Is.EqualTo(1));
            Assert.That(items.Single().Id, Is.EqualTo(3));
        }

        [Test]
        public async Task GetSavedByUserPagedAsync_SoftDeletedListing_IsExcluded()
        {
            var listing = await context.Listings.FindAsync(1);
            listing!.IsDeleted = true;
            await context.SaveChangesAsync();

            context.SavedListings.Add(new SavedListing { UserId = "user-2", ListingId = 1 });
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetSavedByUserPagedAsync("user-2", 1, 10, null);

            Assert.That(total, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSavedByUserPagedAsync_Pagination_ReturnsCorrectSlice()
        {
            for (int i = 3; i <= 7; i++)
            {
                context.Listings.Add(SeedHelpers.MakeListing(i, "user-1", title: $"Listing {i}"));
            }
            await context.SaveChangesAsync();

            for (int i = 1; i <= 7; i++)
                await sut.SaveAsync("user-2", i);

            var (page1, total) = await sut.GetSavedByUserPagedAsync("user-2", 1, 3, null);
            var (page3, _) = await sut.GetSavedByUserPagedAsync("user-2", 3, 3, null);

            Assert.That(total, Is.EqualTo(7));
            Assert.That(page1.Count(), Is.EqualTo(3));
            Assert.That(page3.Count(), Is.EqualTo(1));
        }

        // DeleteByListingIdAsync

        [Test]
        public async Task DeleteByListingIdAsync_RemovesAllSavedEntriesForThatListing()
        {
            await sut.SaveAsync("user-1", 1);
            await sut.SaveAsync("user-2", 1);

            await sut.DeleteByListingIdAsync(1);

            Assert.That(await sut.IsSavedAsync("user-1", 1), Is.False);
            Assert.That(await sut.IsSavedAsync("user-2", 1), Is.False);
        }

        [Test]
        public async Task DeleteByListingIdAsync_DoesNotAffectOtherListings()
        {
            await sut.SaveAsync("user-2", 1);
            await sut.SaveAsync("user-2", 2);

            await sut.DeleteByListingIdAsync(1);

            Assert.That(await sut.IsSavedAsync("user-2", 2), Is.True);
        }
    }
}