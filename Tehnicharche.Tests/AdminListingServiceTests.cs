using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;

namespace Tehnicharche.Tests;

[TestFixture]
public class AdminListingServiceTests
{
    private Mock<IAdminListingRepository> repo;
    private AdminListingService sut;

    [SetUp]
    public void SetUp()
    {
        repo = new Mock<IAdminListingRepository>();
        sut = new AdminListingService(repo.Object);
    }

    // helpers

    static Listing MakeListing(int id = 1, bool isDeleted = false) => new()
    {
        Id = id,
        Title = $"Listing {id}",
        Price = 50m,
        IsDeleted = isDeleted,
        CreatedAt = DateTime.UtcNow,
        Category = new Category { Id = 1, Name = "Cat" },
        Creator = new ApplicationUser { Id = "u1", UserName = "user" },
        CategoryId = 1,
        CreatorId = "u1",
        RegionId = 1,
    };

    void SetupPage(IEnumerable<Listing> items, int total)
        => repo.Setup(r => r.GetAdminFilteredAsync(It.IsAny<string>(), It.IsAny<string?>(), 1, 10))
               .ReturnsAsync((items, total));

    // GetListingsAsync

    [Test]
    public async Task GetListingsAsync_ReturnsListingsWithCounts()
    {
        SetupPage(new[] { MakeListing() }, 1);
        repo.Setup(r => r.GetActiveCountAsync()).ReturnsAsync(1);
        repo.Setup(r => r.GetDeletedCountAsync()).ReturnsAsync(2);

        var result = await sut.GetListingsAsync("all", null, 1);

        Assert.That(result.Listings.Count(), Is.EqualTo(1));
        Assert.That(result.ActiveCount, Is.EqualTo(1));
        Assert.That(result.DeletedCount, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(3));
    }

    [Test]
    public async Task GetListingsAsync_ZeroPageNormalisesToOne()
    {
        SetupPage(Enumerable.Empty<Listing>(), 0);
        repo.Setup(r => r.GetActiveCountAsync()).ReturnsAsync(0);
        repo.Setup(r => r.GetDeletedCountAsync()).ReturnsAsync(0);

        var result = await sut.GetListingsAsync("all", null, 0);

        Assert.That(result.Page, Is.EqualTo(1));
    }

    [Test]
    public async Task GetListingsAsync_CalculatesTotalPagesCorrectly()
    {
        SetupPage(new[] { MakeListing() }, 25);
        repo.Setup(r => r.GetActiveCountAsync()).ReturnsAsync(25);
        repo.Setup(r => r.GetDeletedCountAsync()).ReturnsAsync(0);

        var result = await sut.GetListingsAsync("all", null, 1);

        Assert.That(result.TotalPages, Is.EqualTo(3));
    }

    [Test]
    public async Task GetListingsAsync_EmptyResult_TotalPagesIsOne()
    {
        SetupPage(Enumerable.Empty<Listing>(), 0);
        repo.Setup(r => r.GetActiveCountAsync()).ReturnsAsync(0);
        repo.Setup(r => r.GetDeletedCountAsync()).ReturnsAsync(0);

        var result = await sut.GetListingsAsync("all", null, 1);

        Assert.That(result.TotalPages, Is.EqualTo(1));
    }

    [Test]
    public async Task GetListingsAsync_MapsRowFieldsCorrectly()
    {
        var listing = MakeListing(42);
        SetupPage(new[] { listing }, 1);
        repo.Setup(r => r.GetActiveCountAsync()).ReturnsAsync(1);
        repo.Setup(r => r.GetDeletedCountAsync()).ReturnsAsync(0);

        var row = (await sut.GetListingsAsync("all", null, 1)).Listings.First();

        Assert.That(row.Id, Is.EqualTo(42));
        Assert.That(row.Title, Is.EqualTo(listing.Title));
        Assert.That(row.CreatorName, Is.EqualTo(listing.Creator.UserName));
        Assert.That(row.CategoryName, Is.EqualTo(listing.Category.Name));
        Assert.That(row.Price, Is.EqualTo(listing.Price));
        Assert.That(row.IsDeleted, Is.False);
    }

    // GetRecentAsync

    [Test]
    public async Task GetRecentAsync_ReturnsMappedRows()
    {
        repo.Setup(r => r.GetRecentAdminAsync(5)).ReturnsAsync(new[] { MakeListing(1), MakeListing(2) });

        var result = await sut.GetRecentAsync(5);

        Assert.That(result.Count(), Is.EqualTo(2));
    }

    // SoftDeleteAsync

    [Test]
    public async Task SoftDeleteAsync_Found_CallsSoftDelete()
    {
        var listing = MakeListing();
        repo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(listing);

        await sut.SoftDeleteAsync(1);

        repo.Verify(r => r.SoftDeleteAsync(listing), Times.Once);
    }

    [Test]
    public void SoftDeleteAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdTrackedAsync(99)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.SoftDeleteAsync(99));
    }

    // RestoreAsync

    [Test]
    public async Task RestoreAsync_SetsIsDeletedFalseAndSaves()
    {
        var listing = MakeListing(1, isDeleted: true);
        repo.Setup(r => r.GetByIdDeletedAsync(1)).ReturnsAsync(listing);

        await sut.RestoreAsync(1);

        Assert.That(listing.IsDeleted, Is.False);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void RestoreAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdDeletedAsync(99)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.RestoreAsync(99));
    }

    // HardDeleteAsync

    [Test]
    public async Task HardDeleteAsync_Found_CallsHardDelete()
    {
        var listing = MakeListing();
        repo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(listing);

        await sut.HardDeleteAsync(1);

        repo.Verify(r => r.HardDeleteAsync(listing), Times.Once);
    }

    [Test]
    public void HardDeleteAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdTrackedAsync(99)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.HardDeleteAsync(99));
    }

    // SoftDeleteAllByUserAsync

    [Test]
    public async Task SoftDeleteAllByUserAsync_DelegatesToRepository()
    {
        await sut.SoftDeleteAllByUserAsync("user-1");

        repo.Verify(r => r.SoftDeleteAllByUserAsync("user-1"), Times.Once);
    }
}