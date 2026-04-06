using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;
using Tehnicharche.Services.Core.Interfaces;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Tests;

[TestFixture]
public class AdminUserServiceTests
{
    private Mock<ILogger<AdminUserService>> logger;
    private Mock<IUserManagerWrapper> userManager;
    private Mock<IAdminListingRepository> listingRepo;
    private Mock<IContactMessageRepository> messageRepo;
    private Mock<IGenericRepository<Category>> categoryRepo;
    private AdminUserService sut;

    const string AdminId = "admin-1";
    const string RegularId = "user-1";

    [SetUp]
    public void SetUp()
    {
        logger = new Mock<ILogger<AdminUserService>>();
        userManager = new Mock<IUserManagerWrapper>();
        listingRepo = new Mock<IAdminListingRepository>();
        messageRepo = new Mock<IContactMessageRepository>();
        categoryRepo = new Mock<IGenericRepository<Category>>();
        sut = new AdminUserService(
            userManager.Object,
            listingRepo.Object,
            messageRepo.Object,
            categoryRepo.Object,
            logger.Object);
    }

    // helpers

    static ApplicationUser MakeUser(string id, string name, bool isBanned = false) =>
        new() { Id = id, UserName = name, Email = $"{name}@test.com", IsBanned = isBanned };

    void SetupPagedUsers(IEnumerable<ApplicationUser> users, int total, int banned = 0)
    {
        userManager.Setup(m => m.CountAsync(It.IsAny<string?>())).ReturnsAsync(total);
        userManager.Setup(m => m.CountBannedAsync()).ReturnsAsync(banned);
        userManager.Setup(m => m.GetUsersAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(users.ToList());
        userManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string>());
        listingRepo.Setup(r => r.GetListingCountsByCreatorsAsync())
            .ReturnsAsync(new Dictionary<string, int>());
    }

    // GetUsersAsync

    [Test]
    public async Task GetUsersAsync_ReturnsAllUsers()
    {
        var users = new[] { MakeUser("u1", "gosho"), MakeUser("u2", "pesho") };
        SetupPagedUsers(users, 2);

        var result = await sut.GetUsersAsync(1, null);

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.Users.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetUsersAsync_BannedCount_ReflectsCorrectly()
    {
        SetupPagedUsers(new[] { MakeUser("u1", "gosho", isBanned: true) }, 1, banned: 1);

        var result = await sut.GetUsersAsync(1, null);

        Assert.That(result.BannedCount, Is.EqualTo(1));
        Assert.That(result.Users.First().IsBanned, Is.True);
    }

    [Test]
    public async Task GetUsersAsync_ListingCountForUser_MapsCorrectly()
    {
        var user = MakeUser("u1", "gosho");
        SetupPagedUsers(new[] { user }, 1);
        listingRepo.Setup(r => r.GetListingCountsByCreatorsAsync())
            .ReturnsAsync(new Dictionary<string, int> { { "u1", 7 } });

        var result = await sut.GetUsersAsync(1, null);

        Assert.That(result.Users.First().ListingCount, Is.EqualTo(7));
    }

    [Test]
    public async Task GetUsersAsync_UserRoles_AreIncluded()
    {
        var user = MakeUser("u1", "gosho");
        SetupPagedUsers(new[] { user }, 1);
        userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { AdminRole });

        var result = await sut.GetUsersAsync(1, null);

        Assert.That(result.Users.First().Roles, Does.Contain(AdminRole));
    }

    [Test]
    public async Task GetUsersAsync_ZeroPageNormalisesToOne()
    {
        SetupPagedUsers(Array.Empty<ApplicationUser>(), 0);

        var result = await sut.GetUsersAsync(0, null);

        Assert.That(result.Page, Is.EqualTo(1));
    }

    [Test]
    public async Task GetUsersAsync_SearchTermPassedToWrapper()
    {
        SetupPagedUsers(Array.Empty<ApplicationUser>(), 0);

        await sut.GetUsersAsync(1, "gosho");

        userManager.Verify(m => m.CountAsync("gosho"), Times.Once);
        userManager.Verify(m => m.GetUsersAsync("gosho", 0, It.IsAny<int>()), Times.Once);
    }

    // ToggleRoleAsync

    [Test]
    public async Task ToggleRoleAsync_UserDoesNotHaveRole_AddsIt()
    {
        var user = MakeUser(RegularId, "pesho");
        userManager.Setup(m => m.FindByIdAsync(RegularId)).ReturnsAsync(user);
        userManager.Setup(m => m.IsInRoleAsync(user, UserRole)).ReturnsAsync(false);

        await sut.ToggleRoleAsync(RegularId, UserRole, AdminId);

        userManager.Verify(m => m.AddToRoleAsync(user, UserRole), Times.Once);
        userManager.Verify(m => m.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ToggleRoleAsync_UserAlreadyHasRole_RemovesIt()
    {
        var user = MakeUser(RegularId, "pesho");
        userManager.Setup(m => m.FindByIdAsync(RegularId)).ReturnsAsync(user);
        userManager.Setup(m => m.IsInRoleAsync(user, UserRole)).ReturnsAsync(true);

        await sut.ToggleRoleAsync(RegularId, UserRole, AdminId);

        userManager.Verify(m => m.RemoveFromRoleAsync(user, UserRole), Times.Once);
        userManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void ToggleRoleAsync_AdminRemovingOwnAdminRole_Throws()
    {
        Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ToggleRoleAsync(AdminId, AdminRole, AdminId));
    }

    [Test]
    public void ToggleRoleAsync_UserNotFound_Throws()
    {
        userManager.Setup(m => m.FindByIdAsync("nikoj")).ReturnsAsync((ApplicationUser?)null);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ToggleRoleAsync("nikoj", UserRole, AdminId));
    }

    // BanAsync

    [Test]
    public async Task BanAsync_RegularUser_SetsBannedAndSoftDeletesListings()
    {
        var user = MakeUser(RegularId, "pesho");
        userManager.Setup(m => m.FindByIdAsync(RegularId)).ReturnsAsync(user);
        userManager.Setup(m => m.IsInRoleAsync(user, AdminRole)).ReturnsAsync(false);
        userManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        userManager.Setup(m => m.UpdateSecurityStampAsync(user)).ReturnsAsync(IdentityResult.Success);

        await sut.BanAsync(RegularId);

        Assert.That(user.IsBanned, Is.True);
        listingRepo.Verify(r => r.SoftDeleteAllByUserAsync(RegularId), Times.Once);
    }

    [Test]
    public void BanAsync_AdminUser_Throws()
    {
        var admin = MakeUser(AdminId, "admin");
        userManager.Setup(m => m.FindByIdAsync(AdminId)).ReturnsAsync(admin);
        userManager.Setup(m => m.IsInRoleAsync(admin, AdminRole)).ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.BanAsync(AdminId));
        listingRepo.Verify(r => r.SoftDeleteAllByUserAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void BanAsync_NotFound_Throws()
    {
        userManager.Setup(m => m.FindByIdAsync("nikoj")).ReturnsAsync((ApplicationUser?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.BanAsync("nikoj"));
    }

    // UnbanAsync

    [Test]
    public async Task UnbanAsync_BannedUser_SetsIsBannedFalse()
    {
        var user = MakeUser(RegularId, "pesho", isBanned: true);
        userManager.Setup(m => m.FindByIdAsync(RegularId)).ReturnsAsync(user);
        userManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        userManager.Setup(m => m.UpdateSecurityStampAsync(user)).ReturnsAsync(IdentityResult.Success);

        await sut.UnbanAsync(RegularId);

        Assert.That(user.IsBanned, Is.False);
    }

    [Test]
    public void UnbanAsync_NotFound_Throws()
    {
        userManager.Setup(m => m.FindByIdAsync("nikoj")).ReturnsAsync((ApplicationUser?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.UnbanAsync("nikoj"));
    }
}