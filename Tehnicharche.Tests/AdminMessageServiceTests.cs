using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;

namespace Tehnicharche.Tests;

[TestFixture]
public class AdminMessageServiceTests
{
    private Mock<IContactMessageRepository> repo;
    private AdminMessageService sut;

    [SetUp]
    public void SetUp()
    {
        repo = new Mock<IContactMessageRepository>();
        sut = new AdminMessageService(repo.Object);
    }

    // helpers

    static ContactMessage MakeMessage(int id = 1, bool isRead = false) => new()
    {
        Id = id,
        Name = "Alice",
        Email = "alice@test.com",
        PhoneNumber = "+1234567890",
        Subject = "Hello",
        Message = "Body",
        IsRead = isRead,
        SentAt = DateTime.UtcNow,
    };

    // GetMessagesAsync

    [Test]
    public async Task GetMessagesAsync_ReturnsViewModel()
    {
        repo.Setup(r => r.GetAllAsync("all", 1, 10)).ReturnsAsync((new[] { MakeMessage() }, 1));
        repo.Setup(r => r.GetUnreadCountAsync()).ReturnsAsync(1);
        repo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(1);

        var result = await sut.GetMessagesAsync("all", 1);

        Assert.That(result.Messages.Count(), Is.EqualTo(1));
        Assert.That(result.UnreadCount, Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GetMessagesAsync_ZeroPageNormalisesToOne()
    {
        repo.Setup(r => r.GetAllAsync("all", 1, 10)).ReturnsAsync((Enumerable.Empty<ContactMessage>(), 0));
        repo.Setup(r => r.GetUnreadCountAsync()).ReturnsAsync(0);
        repo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);

        var result = await sut.GetMessagesAsync("all", 0);

        Assert.That(result.Page, Is.EqualTo(1));
    }

    [Test]
    public async Task GetMessagesAsync_CalculatesTotalPages()
    {
        repo.Setup(r => r.GetAllAsync("all", 1, 10)).ReturnsAsync((new[] { MakeMessage() }, 21));
        repo.Setup(r => r.GetUnreadCountAsync()).ReturnsAsync(0);
        repo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(21);

        var result = await sut.GetMessagesAsync("all", 1);

        Assert.That(result.TotalPages, Is.EqualTo(3));
    }

    [Test]
    public async Task GetMessagesAsync_EmptyResult_TotalPagesIsOne()
    {
        repo.Setup(r => r.GetAllAsync(It.IsAny<string>(), 1, 10)).ReturnsAsync((Enumerable.Empty<ContactMessage>(), 0));
        repo.Setup(r => r.GetUnreadCountAsync()).ReturnsAsync(0);
        repo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);

        var result = await sut.GetMessagesAsync("all", 1);

        Assert.That(result.TotalPages, Is.EqualTo(1));
    }

    [Test]
    public async Task GetMessagesAsync_MapsRowFieldsCorrectly()
    {
        var msg = MakeMessage(7, isRead: false);
        repo.Setup(r => r.GetAllAsync(It.IsAny<string>(), 1, 10)).ReturnsAsync((new[] { msg }, 1));
        repo.Setup(r => r.GetUnreadCountAsync()).ReturnsAsync(1);
        repo.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(1);

        var row = (await sut.GetMessagesAsync("all", 1)).Messages.First();

        Assert.That(row.Id, Is.EqualTo(7));
        Assert.That(row.Name, Is.EqualTo(msg.Name));
        Assert.That(row.Email, Is.EqualTo(msg.Email));
        Assert.That(row.Subject, Is.EqualTo(msg.Subject));
        Assert.That(row.IsRead, Is.False);
    }

    // GetByIdAsync

    [Test]
    public async Task GetByIdAsync_UnreadMessage_MarksAsReadAndSaves()
    {
        var msg = MakeMessage(1, isRead: false);
        repo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(msg);

        await sut.GetByIdAsync(1);

        Assert.That(msg.IsRead, Is.True);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_AlreadyRead_DoesNotSaveAgain()
    {
        repo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(MakeMessage(1, isRead: true));

        await sut.GetByIdAsync(1);

        repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public void GetByIdAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdTrackedAsync(99)).ReturnsAsync((ContactMessage?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetByIdAsync(99));
    }

    // GetUnreadCountAsync

    [Test]
    public async Task GetUnreadCountAsync_DelegatesToRepository()
    {
        repo.Setup(r => r.GetUnreadCountAsync()).ReturnsAsync(7);

        Assert.That(await sut.GetUnreadCountAsync(), Is.EqualTo(7));
    }

    // MarkReadAsync/MarkUnreadAsync

    [Test]
    public async Task MarkReadAsync_SetsIsReadTrueAndSaves()
    {
        var msg = MakeMessage(1, isRead: false);
        repo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(msg);

        await sut.MarkReadAsync(1);

        Assert.That(msg.IsRead, Is.True);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void MarkReadAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdTrackedAsync(99)).ReturnsAsync((ContactMessage?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.MarkReadAsync(99));
    }

    [Test]
    public async Task MarkUnreadAsync_SetsIsReadFalseAndSaves()
    {
        var msg = MakeMessage(1, isRead: true);
        repo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(msg);

        await sut.MarkUnreadAsync(1);

        Assert.That(msg.IsRead, Is.False);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void MarkUnreadAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdTrackedAsync(99)).ReturnsAsync((ContactMessage?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.MarkUnreadAsync(99));
    }

    // DeleteAsync

    [Test]
    public async Task DeleteAsync_Found_CallsDeleteOnRepository()
    {
        var msg = MakeMessage();
        repo.Setup(r => r.GetByIdTrackedAsync(1)).ReturnsAsync(msg);

        await sut.DeleteAsync(1);

        repo.Verify(r => r.DeleteAsync(msg), Times.Once);
    }

    [Test]
    public void DeleteAsync_NotFound_Throws()
    {
        repo.Setup(r => r.GetByIdTrackedAsync(99)).ReturnsAsync((ContactMessage?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(99));
    }

    // GetRecentAsync

    [Test]
    public async Task GetRecentAsync_ReturnsMappedRows()
    {
        repo.Setup(r => r.GetRecentAsync(5)).ReturnsAsync(new[] { MakeMessage(1), MakeMessage(2) });

        var result = await sut.GetRecentAsync(5);

        Assert.That(result.Count(), Is.EqualTo(2));
    }
}