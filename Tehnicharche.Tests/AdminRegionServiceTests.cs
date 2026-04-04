using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Tests;

[TestFixture]
public class AdminRegionServiceTests
{
    private Mock<ILogger<AdminRegionService>> logger;
    private Mock<IAdminRegionRepository> repo;
    private IMemoryCache cache;
    private AdminRegionService sut;

    [SetUp]
    public void SetUp()
    {
        logger = new Mock<ILogger<AdminRegionService>>();
        repo = new Mock<IAdminRegionRepository>();
        cache = new MemoryCache(new MemoryCacheOptions());
        sut = new AdminRegionService(repo.Object, cache, logger.Object);
    }

    [TearDown]
    public void TearDown() => cache.Dispose();

    // GetRegionsAsync

    [Test]
    public async Task GetRegionsAsync_ReturnsRegionsWithCounts()
    {
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Region { Id = 1, Name = "Sofia" } });
        repo.Setup(r => r.GetListingCountsAsync()).ReturnsAsync(new Dictionary<int, int> { { 1, 3 } });
        repo.Setup(r => r.GetCityCountsAsync()).ReturnsAsync(new Dictionary<int, int> { { 1, 2 } });

        var row = (await sut.GetRegionsAsync()).Regions.First();

        Assert.That(row.ListingCount, Is.EqualTo(3));
        Assert.That(row.CityCount, Is.EqualTo(2));
    }

    [Test]
    public async Task GetRegionsAsync_NoCountsForRegion_ReturnsZero()
    {
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Region { Id = 5, Name = "New" } });
        repo.Setup(r => r.GetListingCountsAsync()).ReturnsAsync(new Dictionary<int, int>());
        repo.Setup(r => r.GetCityCountsAsync()).ReturnsAsync(new Dictionary<int, int>());

        var row = (await sut.GetRegionsAsync()).Regions.First();

        Assert.That(row.ListingCount, Is.EqualTo(0));
        Assert.That(row.CityCount, Is.EqualTo(0));
    }

    // AddAsync

    [Test]
    public async Task AddAsync_NewName_AddsAndSaves()
    {
        repo.Setup(r => r.NameExistsAsync("Varna", null)).ReturnsAsync(false);

        await sut.AddAsync("Varna");

        repo.Verify(r => r.AddAsync(It.Is<Region>(r => r.Name == "Varna")), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task AddAsync_TrimsWhitespace()
    {
        repo.Setup(r => r.NameExistsAsync("Varna", null)).ReturnsAsync(false);

        await sut.AddAsync("  Varna  ");

        repo.Verify(r => r.AddAsync(It.Is<Region>(r => r.Name == "Varna")), Times.Once);
    }

    [Test]
    public void AddAsync_DuplicateName_Throws()
    {
        repo.Setup(r => r.NameExistsAsync("Sofia", null)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddAsync("Sofia"));
    }

    // GetForEditAsync

    [Test]
    public async Task GetForEditAsync_Found_ReturnsViewModel()
    {
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Region { Id = 1, Name = "Plovdiv" });

        var result = await sut.GetForEditAsync(1);

        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Plovdiv"));
    }

    [Test]
    public void GetForEditAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Region?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetForEditAsync(99));
    }

    // UpdateAsync

    [Test]
    public async Task UpdateAsync_ValidName_UpdatesAndSaves()
    {
        var region = new Region { Id = 1, Name = "Old" };
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(region);
        repo.Setup(r => r.NameExistsAsync("New", 1)).ReturnsAsync(false);

        await sut.UpdateAsync(new EditRegionViewModel { Id = 1, Name = "New" });

        Assert.That(region.Name, Is.EqualTo("New"));
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void UpdateAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Region?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.UpdateAsync(new EditRegionViewModel { Id = 99, Name = "X" }));
    }

    [Test]
    public void UpdateAsync_DuplicateName_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Region { Id = 1, Name = "Old" });
        repo.Setup(r => r.NameExistsAsync("Taken", 1)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.UpdateAsync(new EditRegionViewModel { Id = 1, Name = "Taken" }));
    }

    // DeleteAsync

    [Test]
    public async Task DeleteAsync_NotInUse_DeletesAndSaves()
    {
        var region = new Region { Id = 1, Name = "Empty" };
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(region);
        repo.Setup(r => r.IsInUseAsync(1)).ReturnsAsync(false);

        await sut.DeleteAsync(1);

        repo.Verify(r => r.DeleteAsync(region), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void DeleteAsync_InUse_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Region { Id = 1, Name = "InUse" });
        repo.Setup(r => r.IsInUseAsync(1)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(1));
    }

    [Test]
    public void DeleteAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Region?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(99));
    }
}