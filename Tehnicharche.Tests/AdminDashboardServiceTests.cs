using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;
using Tehnicharche.Services.Core.Interfaces;

namespace Tehnicharche.Tests;

[TestFixture]
public class AdminDashboardServiceTest
{
    private Mock<IUserManagerWrapper> userManager;
    private Mock<IAdminListingRepository> listingRepo;
    private Mock<IContactMessageRepository> messageRepo;
    private Mock<IGenericRepository<Category>> categoryRepo;
    private AdminDashboardService sut;

    [SetUp]
    public void SetUp()
    {
        userManager = new Mock<IUserManagerWrapper>();
        listingRepo = new Mock<IAdminListingRepository>();
        messageRepo = new Mock<IContactMessageRepository>();
        categoryRepo = new Mock<IGenericRepository<Category>>();
        sut = new AdminDashboardService(
            userManager.Object,
            listingRepo.Object,
            messageRepo.Object,
            categoryRepo.Object);
    }

   
    [Test]
    public async Task GetDashboardStatsAsync_AggregatesAllStats()
    {
        listingRepo.Setup(r => r.GetActiveCountAsync()).ReturnsAsync(10);
        listingRepo.Setup(r => r.GetDeletedCountAsync()).ReturnsAsync(2);
        listingRepo.Setup(r => r.GetRecentAdminAsync(It.IsAny<int>())).ReturnsAsync(Enumerable.Empty<Listing>());
        messageRepo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(5);
        messageRepo.Setup(r => r.GetUnreadCountAsync()).ReturnsAsync(3);
        messageRepo.Setup(r => r.GetRecentAsync(It.IsAny<int>())).ReturnsAsync(Enumerable.Empty<ContactMessage>());
        categoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { new Category(), new Category(), new Category() });
        userManager.Setup(m => m.CountAsync(null)).ReturnsAsync(42);

        var result = await sut.GetDashboardStatsAsync();

        Assert.That(result.TotalActiveListings, Is.EqualTo(10));
        Assert.That(result.TotalDeletedListings, Is.EqualTo(2));
        Assert.That(result.TotalMessages, Is.EqualTo(5));
        Assert.That(result.UnreadMessages, Is.EqualTo(3));
        Assert.That(result.TotalCategories, Is.EqualTo(3));
        Assert.That(result.TotalUsers, Is.EqualTo(42));
    }
}