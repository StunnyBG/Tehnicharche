using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Tests;

[TestFixture]
public class SavedListingServiceTests
{
    private Mock<ISavedListingRepository> repo;
    private SavedListingService sut;

    const string UserId = "user-abc";

    [SetUp]
    public void SetUp()
    {
        repo = new Mock<ISavedListingRepository>();
        sut = new SavedListingService(repo.Object);
    }

    // helpers

    static Listing MakeListing(int id = 1) => new()
    {
        Id = id,
        Title = $"Listing {id}",
        Price = 99m,
        CategoryId = 1,
        RegionId = 1,
        Category = new Category { Id = 1, Name = "Category" },
        Region = new Region { Id = 1, Name = "Region" },
        City = new City { Id = 1, Name = "City", RegionId = 1 },
        CreatorId = "x",
    };

    // IsSavedAsync

    [Test]
    public async Task IsSavedAsync_Saved_ReturnsTrue()
    {
        repo.Setup(r => r.IsSavedAsync(UserId, 5)).ReturnsAsync(true);

        Assert.That(await sut.IsSavedAsync(UserId, 5), Is.True);
    }

    [Test]
    public async Task IsSavedAsync_NotSaved_ReturnsFalse()
    {
        repo.Setup(r => r.IsSavedAsync(UserId, 5)).ReturnsAsync(false);

        Assert.That(await sut.IsSavedAsync(UserId, 5), Is.False);
    }

    // ToggleSaveAsync

    [Test]
    public async Task ToggleSaveAsync_NotYetSaved_CallsSave()
    {
        repo.Setup(r => r.IsSavedAsync(UserId, 3)).ReturnsAsync(false);

        await sut.ToggleSaveAsync(UserId, 3);

        repo.Verify(r => r.SaveAsync(UserId, 3), Times.Once);
        repo.Verify(r => r.UnsaveAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task ToggleSaveAsync_AlreadySaved_CallsUnsave()
    {
        repo.Setup(r => r.IsSavedAsync(UserId, 3)).ReturnsAsync(true);

        await sut.ToggleSaveAsync(UserId, 3);

        repo.Verify(r => r.UnsaveAsync(UserId, 3), Times.Once);
        repo.Verify(r => r.SaveAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    // GetSavedListingsAsync

    [Test]
    public async Task GetSavedListingsAsync_ReturnsListingsAndTotal()
    {
        repo.Setup(r => r.GetSavedByUserPagedAsync(UserId, 1, It.IsAny<int>(), null))
            .ReturnsAsync((new[] { MakeListing() }, 1));

        var result = await sut.GetSavedListingsAsync(new SavedListingsQueryModel(), UserId);

        Assert.That(result.TotalListings, Is.EqualTo(1));
        Assert.That(result.Listings.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetSavedListingsAsync_ZeroPageNormalisesToOne()
    {
        repo.Setup(r => r.GetSavedByUserPagedAsync(UserId, 1, It.IsAny<int>(), null))
            .ReturnsAsync((Enumerable.Empty<Listing>(), 0));

        var result = await sut.GetSavedListingsAsync(new SavedListingsQueryModel { Page = 0 }, UserId);

        Assert.That(result.Page, Is.EqualTo(1));
    }

    [Test]
    public async Task GetSavedListingsAsync_NullCity_ReturnNullCityName()
    {
        var listing = MakeListing();
        listing.City = null;
        repo.Setup(r => r.GetSavedByUserPagedAsync(UserId, 1, It.IsAny<int>(), null))
            .ReturnsAsync((new[] { listing }, 1));

        var result = await sut.GetSavedListingsAsync(new SavedListingsQueryModel(), UserId);

        Assert.That(result.Listings.First().CityName, Is.Null);
    }

    [Test]
    public async Task GetSavedListingsAsync_PassesSearchTermToRepository()
    {
        repo.Setup(r => r.GetSavedByUserPagedAsync(UserId, 1, It.IsAny<int>(), "soldering"))
            .ReturnsAsync((Enumerable.Empty<Listing>(), 0));

        await sut.GetSavedListingsAsync(new SavedListingsQueryModel { SearchTerm = "soldering" }, UserId);

        repo.Verify(r => r.GetSavedByUserPagedAsync(UserId, 1, It.IsAny<int>(), "soldering"), Times.Once);
    }

    [Test]
    public async Task GetSavedListingsAsync_MapsListingFields()
    {
        var listing = MakeListing(42);
        repo.Setup(r => r.GetSavedByUserPagedAsync(UserId, 1, It.IsAny<int>(), null))
            .ReturnsAsync((new[] { listing }, 1));

        var vm = (await sut.GetSavedListingsAsync(new SavedListingsQueryModel(), UserId)).Listings.First();

        Assert.That(vm.Id, Is.EqualTo(42));
        Assert.That(vm.CategoryName, Is.EqualTo(listing.Category.Name));
        Assert.That(vm.RegionName, Is.EqualTo(listing.Region.Name));
    }
}