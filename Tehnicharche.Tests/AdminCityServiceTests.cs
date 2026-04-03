using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;
using Tehnicharche.ViewModels.Admin;

namespace Tehnicharche.Tests;

[TestFixture]
public class AdminCityServiceTests
{
    private Mock<IAdminCityRepository> cityRepo;
    private Mock<IAdminRegionRepository> regionRepo;
    private IMemoryCache cache;
    private AdminCityService sut;

    [SetUp]
    public void SetUp()
    {
        cityRepo = new Mock<IAdminCityRepository>();
        regionRepo = new Mock<IAdminRegionRepository>();
        cache = new MemoryCache(new MemoryCacheOptions());
        sut = new AdminCityService(cityRepo.Object, regionRepo.Object, cache);
    }

    [TearDown]
    public void TearDown() => cache.Dispose();

    // GetCitiesAsync

    [Test]
    public async Task GetCitiesAsync_ResolvesRegionName()
    {
        cityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new City { Id = 1, Name = "Sofia", RegionId = 23 } });
        regionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Region { Id = 23, Name = "Sofia City" } });
        cityRepo.Setup(r => r.GetListingCountsAsync()).ReturnsAsync(new Dictionary<int, int> { { 1, 10 } });

        var city = (await sut.GetCitiesAsync()).Cities.First();

        Assert.That(city.Name, Is.EqualTo("Sofia"));
        Assert.That(city.RegionName, Is.EqualTo("Sofia City"));
        Assert.That(city.ListingCount, Is.EqualTo(10));
    }

    [Test]
    public async Task GetCitiesAsync_UnknownRegionId_ReturnsDash()
    {
        cityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new City { Id = 1, Name = "Orphan", RegionId = 999 } });
        regionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<Region>());
        cityRepo.Setup(r => r.GetListingCountsAsync()).ReturnsAsync(new Dictionary<int, int>());

        var city = (await sut.GetCitiesAsync()).Cities.First();

        Assert.That(city.RegionName, Is.EqualTo("—"));
    }

    [Test]
    public async Task GetCitiesAsync_ReturnsAvailableRegions()
    {
        cityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<City>());
        regionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Region { Id = 1, Name = "Burgas" } });
        cityRepo.Setup(r => r.GetListingCountsAsync()).ReturnsAsync(new Dictionary<int, int>());

        var result = await sut.GetCitiesAsync();

        Assert.That(result.AvailableRegions.Count(), Is.EqualTo(1));
    }

    // AddAsync

    [Test]
    public async Task AddAsync_NewCity_AddsAndSaves()
    {
        cityRepo.Setup(r => r.NameExistsInRegionAsync("Burgas", 2, null)).ReturnsAsync(false);

        await sut.AddAsync("Burgas", 2);

        cityRepo.Verify(r => r.AddAsync(It.Is<City>(c => c.Name == "Burgas" && c.RegionId == 2)), Times.Once);
        cityRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task AddAsync_TrimsWhitespace()
    {
        cityRepo.Setup(r => r.NameExistsInRegionAsync("Burgas", 2, null)).ReturnsAsync(false);

        await sut.AddAsync("  Burgas  ", 2);

        cityRepo.Verify(r => r.AddAsync(It.Is<City>(c => c.Name == "Burgas")), Times.Once);
    }

    [Test]
    public void AddAsync_DuplicateCityInRegion_Throws()
    {
        cityRepo.Setup(r => r.NameExistsInRegionAsync("Varna", 3, null)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddAsync("Varna", 3));
    }

    // GetForEditAsync

    [Test]
    public async Task GetForEditAsync_Found_ReturnsViewModelWithRegions()
    {
        cityRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1, Name = "Plovdiv", RegionId = 16 });
        regionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Region { Id = 16, Name = "Plovdiv Province" } });

        var result = await sut.GetForEditAsync(1);

        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.RegionId, Is.EqualTo(16));
        Assert.That(result.Regions.Count(), Is.EqualTo(1));
    }

    [Test]
    public void GetForEditAsync_NotFound_Throws()
    {
        cityRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((City?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetForEditAsync(99));
    }

    // UpdateAsync

    [Test]
    public async Task UpdateAsync_ValidChange_UpdatesNameAndRegion()
    {
        var city = new City { Id = 1, Name = "Old", RegionId = 1 };
        cityRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(city);
        cityRepo.Setup(r => r.NameExistsInRegionAsync("New", 2, 1)).ReturnsAsync(false);

        await sut.UpdateAsync(new EditCityViewModel { Id = 1, Name = "New", RegionId = 2 });

        Assert.That(city.Name, Is.EqualTo("New"));
        Assert.That(city.RegionId, Is.EqualTo(2));
        cityRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void UpdateAsync_NotFound_Throws()
    {
        cityRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((City?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.UpdateAsync(new EditCityViewModel { Id = 99, Name = "X", RegionId = 1 }));
    }

    [Test]
    public void UpdateAsync_DuplicateNameInRegion_Throws()
    {
        cityRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1, Name = "Old", RegionId = 1 });
        cityRepo.Setup(r => r.NameExistsInRegionAsync("Taken", 1, 1)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.UpdateAsync(new EditCityViewModel { Id = 1, Name = "Taken", RegionId = 1 }));
    }

    // DeleteAsync

    [Test]
    public async Task DeleteAsync_NotInUse_DeletesAndSaves()
    {
        var city = new City { Id = 1, Name = "Empty", RegionId = 1 };
        cityRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(city);
        cityRepo.Setup(r => r.IsInUseAsync(1)).ReturnsAsync(false);

        await sut.DeleteAsync(1);

        cityRepo.Verify(r => r.DeleteAsync(city), Times.Once);
        cityRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void DeleteAsync_InUse_Throws()
    {
        cityRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1, Name = "Used", RegionId = 1 });
        cityRepo.Setup(r => r.IsInUseAsync(1)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(1));
    }

    [Test]
    public void DeleteAsync_NotFound_Throws()
    {
        cityRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((City?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(99));
    }
}