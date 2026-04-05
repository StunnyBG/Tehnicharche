using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;
using Tehnicharche.ViewModels.Listing;

namespace Tehnicharche.Tests;

[TestFixture]
public class ListingServiceTests
{
    private Mock<ILogger<ListingService>> logger;
    private Mock<IListingRepository> listingRepo;
    private Mock<IGenericRepository<Category>> categoryRepo;
    private Mock<IGenericRepository<Region>> regionRepo;
    private Mock<IGenericRepository<City>> cityRepo;
    private IMemoryCache cache;
    private ListingService sut;

    [SetUp]
    public void SetUp()
    {
        logger = new Mock<ILogger<ListingService>>();
        listingRepo = new Mock<IListingRepository>();
        categoryRepo = new Mock<IGenericRepository<Category>>();
        regionRepo = new Mock<IGenericRepository<Region>>();
        cityRepo = new Mock<IGenericRepository<City>>();
        cache = new MemoryCache(new MemoryCacheOptions());
        sut = new ListingService(
            listingRepo.Object,
            categoryRepo.Object,
            regionRepo.Object,
            cityRepo.Object,
            cache,
            logger.Object);
    }

    [TearDown]
    public void TearDown() => cache.Dispose();

    // helpers

    static Listing MakeListing(int id = 1, string creatorId = "user-1") => new()
    {
        Id = id,
        Title = $"Listing {id}",
        Price = 10m,
        CategoryId = 1,
        RegionId = 1,
        Category = new Category { Id = 1, Name = "Category" },
        Region = new Region { Id = 1, Name = "Region" },
        City = new City { Id = 1, Name = "City", RegionId = 1 },
        CreatorId = creatorId,
        Creator = new ApplicationUser { Id = creatorId, UserName = "user", Email = "u@e.com" },
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    void SetupLookups()
    {
        categoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category> { new() { Id = 1, Name = "Category" } });
        regionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Region> { new() { Id = 1, Name = "Region" } });
        cityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<City> { new() { Id = 1, Name = "City", RegionId = 1 } });
    }

    void SetupValidCategoryAndRegion()
    {
        categoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(true);
        regionRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Region, bool>>>())).ReturnsAsync(true);
    }

    // GetIndexListingsAsync

    [Test]
    public async Task GetIndexListingsAsync_ReturnsPopulatedQueryModel()
    {
        SetupLookups();
        listingRepo.Setup(r => r.GetFilteredPagedAsync(1, It.IsAny<int>(), null, null, null, null, null, null))
            .ReturnsAsync((new List<Listing> { MakeListing() }, 1));

        var result = await sut.GetIndexListingsAsync(new ListingIndexQueryModel());

        Assert.That(result.TotalListings, Is.EqualTo(1));
        Assert.That(result.Listings.Count(), Is.EqualTo(1));
        Assert.That(result.Categories.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetIndexListingsAsync_ZeroPageNormalisesToOne()
    {
        SetupLookups();
        listingRepo.Setup(r => r.GetFilteredPagedAsync(1, It.IsAny<int>(), null, null, null, null, null, null))
            .ReturnsAsync((Enumerable.Empty<Listing>(), 0));

        var result = await sut.GetIndexListingsAsync(new ListingIndexQueryModel { Page = 0 });

        Assert.That(result.Page, Is.EqualTo(1));
    }

    [Test]
    public async Task GetIndexListingsAsync_MapsListingFieldsCorrectly()
    {
        SetupLookups();
        var listing = MakeListing();
        listingRepo.Setup(r => r.GetFilteredPagedAsync(It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<string?>()))
            .ReturnsAsync((new List<Listing> { listing }, 1));

        var vm = (await sut.GetIndexListingsAsync(new ListingIndexQueryModel())).Listings.First();

        Assert.That(vm.Id, Is.EqualTo(listing.Id));
        Assert.That(vm.Title, Is.EqualTo(listing.Title));
        Assert.That(vm.CategoryName, Is.EqualTo(listing.Category.Name));
        Assert.That(vm.RegionName, Is.EqualTo(listing.Region.Name));
        Assert.That(vm.CityName, Is.EqualTo(listing.City!.Name));
    }

    // GetMyListingsAsync

    [Test]
    public async Task GetMyListingsAsync_ReturnsOnlyCreatorListings()
    {
        var listing = MakeListing(1, "user-1");
        listingRepo.Setup(r => r.GetByCreatorPagedAsync("user-1", 1, It.IsAny<int>(), null))
            .ReturnsAsync((new List<Listing> { listing }, 1));

        var result = await sut.GetMyListingsAsync(new MyListingsQueryModel(), "user-1");

        Assert.That(result.TotalListings, Is.EqualTo(1));
        Assert.That(result.Listings.First().Id, Is.EqualTo(listing.Id));
    }

    [Test]
    public async Task GetMyListingsAsync_ZeroPageNormalisesToOne()
    {
        listingRepo.Setup(r => r.GetByCreatorPagedAsync("user-1", 1, It.IsAny<int>(), null))
            .ReturnsAsync((Enumerable.Empty<Listing>(), 0));

        var result = await sut.GetMyListingsAsync(new MyListingsQueryModel { Page = 0 }, "user-1");

        Assert.That(result.Page, Is.EqualTo(1));
    }

    // GetListingDetailsByIdAsync

    [Test]
    public async Task GetListingDetailsByIdAsync_ReturnsDetailsViewModel()
    {
        var listing = MakeListing();
        listingRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(listing);

        var result = await sut.GetListingDetailsByIdAsync(1);

        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.CreatorName, Is.EqualTo(listing.Creator.UserName));
        Assert.That(result.CreatorEmail, Is.EqualTo(listing.Creator.Email));
    }

    [Test]
    public void GetListingDetailsByIdAsync_NotFound_Throws()
    {
        listingRepo.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetListingDetailsByIdAsync(99));
    }

    [Test]
    public async Task GetListingDetailsByIdAsync_NullCity_ReturnsEmptyString()
    {
        var listing = MakeListing();
        listing.City = null;
        listingRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(listing);

        var result = await sut.GetListingDetailsByIdAsync(1);

        Assert.That(result.CityName, Is.EqualTo(string.Empty));
    }

    // AddListingAsync

    [Test]
    public async Task AddListingAsync_ValidModel_CallsAddAndSave()
    {
        SetupValidCategoryAndRegion();
        var model = new ListingCreateViewModel { Title = "Title", Price = 10m, CategoryId = 1, RegionId = 1 };

        await sut.AddListingAsync(model, "user-1");

        listingRepo.Verify(r => r.AddAsync(It.IsAny<Listing>()), Times.Once);
        listingRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void AddListingAsync_InvalidCategory_Throws()
    {
        categoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(false);
        var model = new ListingCreateViewModel { Title = "Title", Price = 10m, CategoryId = 99, RegionId = 1 };

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddListingAsync(model, "user-1"));
    }

    [Test]
    public void AddListingAsync_InvalidRegion_Throws()
    {
        categoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(true);
        regionRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Region, bool>>>())).ReturnsAsync(false);
        var model = new ListingCreateViewModel { Title = "Title", Price = 10m, CategoryId = 1, RegionId = 99 };

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddListingAsync(model, "user-1"));
    }

    [Test]
    public void AddListingAsync_CityBelongsToDifferentRegion_Throws()
    {
        SetupValidCategoryAndRegion();
        cityRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new City { Id = 5, RegionId = 2 });
        var model = new ListingCreateViewModel { Title = "Title", Price = 10m, CategoryId = 1, RegionId = 1, CityId = 5 };

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddListingAsync(model, "user-1"));
    }

    [Test]
    public void AddListingAsync_CityNotFound_Throws()
    {
        SetupValidCategoryAndRegion();
        cityRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((City?)null);
        var model = new ListingCreateViewModel { Title = "Title", Price = 10m, CategoryId = 1, RegionId = 1, CityId = 99 };

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddListingAsync(model, "user-1"));
    }

    [Test]
    public async Task AddListingAsync_ValidCityInSameRegion_Succeeds()
    {
        SetupValidCategoryAndRegion();
        cityRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1, RegionId = 1 });
        var model = new ListingCreateViewModel { Title = "Title", Price = 10m, CategoryId = 1, RegionId = 1, CityId = 1 };

        await sut.AddListingAsync(model, "user-1");

        listingRepo.Verify(r => r.AddAsync(It.IsAny<Listing>()), Times.Once);
    }

    // GetListingEditAsync

    [Test]
    public async Task GetListingEditAsync_Owner_ReturnsViewModel()
    {
        SetupLookups();
        listingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeListing(1, "user-1"));

        var result = await sut.GetListingEditAsync(1, "user-1");

        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void GetListingEditAsync_NotFound_Throws()
    {
        listingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetListingEditAsync(1, "user-1"));
    }

    [Test]
    public void GetListingEditAsync_WrongUser_ThrowsUnauthorized()
    {
        listingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeListing(1, "user-2"));

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.GetListingEditAsync(1, "user-1"));
    }

    // EditListingAsync

    [Test]
    public async Task EditListingAsync_Owner_UpdatesFieldsAndSaves()
    {
        var listing = MakeListing(1, "user-1");
        listingRepo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(listing);
        SetupValidCategoryAndRegion();

        await sut.EditListingAsync(new ListingEditViewModel { Id = 1, Title = "Updated", Price = 20m, CategoryId = 1, RegionId = 1 }, "user-1");

        Assert.That(listing.Title, Is.EqualTo("Updated"));
        Assert.That(listing.Price, Is.EqualTo(20m));
        listingRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task EditListingAsync_UpdatesUpdatedAt()
    {
        var listing = MakeListing(1, "user-1");
        listing.UpdatedAt = DateTime.UtcNow.AddDays(-1);
        listingRepo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(listing);
        SetupValidCategoryAndRegion();

        await sut.EditListingAsync(new ListingEditViewModel { Id = 1, Title = "Title", Price = 5m, CategoryId = 1, RegionId = 1 }, "user-1");

        Assert.That(listing.UpdatedAt, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-5)));
    }

    [Test]
    public void EditListingAsync_NotFound_Throws()
    {
        listingRepo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.EditListingAsync(new ListingEditViewModel { Id = 1, CategoryId = 1, RegionId = 1 }, "user-1"));
    }

    [Test]
    public void EditListingAsync_WrongUser_ThrowsUnauthorized()
    {
        listingRepo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(MakeListing(1, "other"));

        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            sut.EditListingAsync(new ListingEditViewModel { Id = 1, CategoryId = 1, RegionId = 1 }, "user-1"));
    }

    // DeleteListingAsync

    [Test]
    public async Task DeleteListingAsync_Owner_CallsSoftDelete()
    {
        var listing = MakeListing(1, "user-1");
        listingRepo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(listing);

        await sut.DeleteListingAsync(1, "user-1");

        listingRepo.Verify(r => r.SoftDeleteAsync(listing), Times.Once);
    }

    [Test]
    public void DeleteListingAsync_NotFound_Throws()
    {
        listingRepo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteListingAsync(1, "user-1"));
    }

    [Test]
    public void DeleteListingAsync_WrongUser_ThrowsUnauthorized()
    {
        listingRepo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(MakeListing(1, "user-2"));

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.DeleteListingAsync(1, "user-1"));
    }

    // GetListingDeleteDetailsAsync

    [Test]
    public async Task GetListingDeleteDetailsAsync_Owner_ReturnsViewModel()
    {
        listingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeListing(1, "user-1"));

        var result = await sut.GetListingDeleteDetailsAsync(1, "user-1");

        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void GetListingDeleteDetailsAsync_NotFound_Throws()
    {
        listingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetListingDeleteDetailsAsync(1, "user-1"));
    }

    [Test]
    public void GetListingDeleteDetailsAsync_WrongUser_ThrowsUnauthorized()
    {
        listingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeListing(1, "user-2"));

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.GetListingDeleteDetailsAsync(1, "user-1"));
    }

    // Caching

    [Test]
    public async Task GetAllCategoriesAsync_SecondCall_HitsRepositoryOnlyOnce()
    {
        categoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category> { new() { Id = 1, Name = "Category" } });

        await sut.GetAllCategoriesAsync();
        await sut.GetAllCategoriesAsync();

        categoryRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllRegionsAsync_SecondCall_HitsRepositoryOnlyOnce()
    {
        regionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Region> { new() { Id = 1, Name = "Region" } });

        await sut.GetAllRegionsAsync();
        await sut.GetAllRegionsAsync();

        regionRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllCitiesAsync_SecondCall_HitsRepositoryOnlyOnce()
    {
        cityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<City> { new() { Id = 1, Name = "City", RegionId = 1 } });

        await sut.GetAllCitiesAsync();
        await sut.GetAllCitiesAsync();

        cityRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllCitiesAsync_MapsRegionIdCorrectly()
    {
        cityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<City> { new() { Id = 7, Name = "Sofia", RegionId = 23 } });

        var result = await sut.GetAllCitiesAsync();

        Assert.That(result.First().RegionId, Is.EqualTo(23));
    }
}