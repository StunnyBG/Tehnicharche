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
public class AdminCategoryServiceTests
{
    private Mock<ILogger<AdminCategoryService>> logger;
    private Mock<IAdminCategoryRepository> repo;
    private IMemoryCache cache;
    private AdminCategoryService sut;

    [SetUp]
    public void SetUp()
    {
        logger = new Mock<ILogger<AdminCategoryService>>();
        repo = new Mock<IAdminCategoryRepository>();
        cache = new MemoryCache(new MemoryCacheOptions());
        sut = new AdminCategoryService(repo.Object, cache, logger.Object);
    }

    [TearDown]
    public void TearDown() => cache.Dispose();

    // GetCategoriesAsync

    [Test]
    public async Task GetCategoriesAsync_ReturnsAllCategoriesWithListingCounts()
    {
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Category { Id = 1, Name = "Category" } });
        repo.Setup(r => r.GetListingCountsAsync()).ReturnsAsync(new Dictionary<int, int> { { 1, 5 } });

        var result = await sut.GetCategoriesAsync();

        Assert.That(result.Categories.Count(), Is.EqualTo(1));
        Assert.That(result.Categories.First().ListingCount, Is.EqualTo(5));
    }

    [Test]
    public async Task GetCategoriesAsync_CategoryNotInCountsDict_ReturnsZero()
    {
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Category { Id = 2, Name = "Category" } });
        repo.Setup(r => r.GetListingCountsAsync()).ReturnsAsync(new Dictionary<int, int>());

        var result = await sut.GetCategoriesAsync();

        Assert.That(result.Categories.First().ListingCount, Is.EqualTo(0));
    }

    // AddAsync

    [Test]
    public async Task AddAsync_NewName_AddsAndSaves()
    {
        repo.Setup(r => r.NameExistsAsync("NewCat", null)).ReturnsAsync(false);

        await sut.AddAsync("NewCat");

        repo.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "NewCat")), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task AddAsync_TrimsWhitespace()
    {
        repo.Setup(r => r.NameExistsAsync("NewCat", null)).ReturnsAsync(false);

        await sut.AddAsync("  NewCat  ");

        repo.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "NewCat")), Times.Once);
    }

    [Test]
    public void AddAsync_DuplicateName_Throws()
    {
        repo.Setup(r => r.NameExistsAsync("Existing", null)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddAsync("Existing"));
        repo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    // GetForEditAsync

    [Test]
    public async Task GetForEditAsync_Found_ReturnsViewModel()
    {
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1, Name = "Category" });

        var result = await sut.GetForEditAsync(1);

        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Category"));
    }

    [Test]
    public void GetForEditAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetForEditAsync(99));
    }

    // UpdateAsync

    [Test]
    public async Task UpdateAsync_ValidName_UpdatesAndSaves()
    {
        var cat = new Category { Id = 1, Name = "OldName" };
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cat);
        repo.Setup(r => r.NameExistsAsync("NewName", 1)).ReturnsAsync(false);

        await sut.UpdateAsync(new EditCategoryViewModel { Id = 1, Name = "NewName" });

        Assert.That(cat.Name, Is.EqualTo("NewName"));
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_TrimsWhitespace()
    {
        var cat = new Category { Id = 1, Name = "Old" };
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cat);
        repo.Setup(r => r.NameExistsAsync("Trimmed", 1)).ReturnsAsync(false);

        await sut.UpdateAsync(new EditCategoryViewModel { Id = 1, Name = "  Trimmed  " });

        Assert.That(cat.Name, Is.EqualTo("Trimmed"));
    }

    [Test]
    public void UpdateAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.UpdateAsync(new EditCategoryViewModel { Id = 99, Name = "X" }));
    }

    [Test]
    public void UpdateAsync_DuplicateName_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1, Name = "Category" });
        repo.Setup(r => r.NameExistsAsync("Duplicate", 1)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.UpdateAsync(new EditCategoryViewModel { Id = 1, Name = "Duplicate" }));
    }

    // DeleteAsync

    [Test]
    public async Task DeleteAsync_NotInUse_DeletesAndSaves()
    {
        var cat = new Category { Id = 1, Name = "Category" };
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cat);
        repo.Setup(r => r.IsInUseAsync(1)).ReturnsAsync(false);

        await sut.DeleteAsync(1);

        repo.Verify(r => r.DeleteAsync(cat), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void DeleteAsync_InUse_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1, Name = "InUse" });
        repo.Setup(r => r.IsInUseAsync(1)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(1));
        repo.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never);
    }

    [Test]
    public void DeleteAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(99));
    }
}