using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Tests;

[TestFixture]
public class ContactServiceTests
{
    private Mock<ILogger<ContactService>> logger;
    private Mock<IContactMessageRepository> repo;
    private ContactService sut;

    [SetUp]
    public void SetUp()
    {
        logger = new Mock<ILogger<ContactService>>();
        repo = new Mock<IContactMessageRepository>();
        sut = new ContactService(repo.Object, logger.Object);
    }

    // helpers

    static ContactFormViewModel MakeViewModel(
        string name = "Alice",
        string email = "alice@example.com",
        string subject = "Hello",
        string message = "Body text here",
        string? phone = null) =>
        new()
        {
            Name = name,
            Email = email,
            Subject = subject,
            Message = message,
            PhoneNumber = phone
        };

    // SubmitAsync

    [Test]
    public async Task SubmitAsync_CallsAddAsync_WithMappedEntity()
    {
        ContactMessage? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<ContactMessage>()))
            .Callback<ContactMessage>(m => captured = m)
            .Returns(Task.CompletedTask);

        await sut.SubmitAsync(MakeViewModel());

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.Name, Is.EqualTo("Alice"));
        Assert.That(captured.Email, Is.EqualTo("alice@example.com"));
        Assert.That(captured.Subject, Is.EqualTo("Hello"));
        Assert.That(captured.Message, Is.EqualTo("Body text here"));
    }

    [Test]
    public async Task SubmitAsync_CallsSaveChangesAsync()
    {
        await sut.SubmitAsync(MakeViewModel());

        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task SubmitAsync_SetsIsReadFalse()
    {
        ContactMessage? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<ContactMessage>()))
            .Callback<ContactMessage>(m => captured = m)
            .Returns(Task.CompletedTask);

        await sut.SubmitAsync(MakeViewModel());

        Assert.That(captured!.IsRead, Is.False);
    }

    [Test]
    public async Task SubmitAsync_SetsSentAtToUtcNow()
    {
        var before = DateTime.UtcNow;
        ContactMessage? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<ContactMessage>()))
            .Callback<ContactMessage>(m => captured = m)
            .Returns(Task.CompletedTask);

        await sut.SubmitAsync(MakeViewModel());

        Assert.That(captured!.SentAt, Is.GreaterThanOrEqualTo(before));
        Assert.That(captured.SentAt, Is.LessThanOrEqualTo(DateTime.UtcNow));
    }

    [Test]
    public async Task SubmitAsync_WithOptionalPhone_MapsPhoneNumber()
    {
        ContactMessage? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<ContactMessage>()))
            .Callback<ContactMessage>(m => captured = m)
            .Returns(Task.CompletedTask);

        await sut.SubmitAsync(MakeViewModel(phone: "+359888000000"));

        Assert.That(captured!.PhoneNumber, Is.EqualTo("+359888000000"));
    }

    [Test]
    public async Task SubmitAsync_WithoutPhone_PhoneNumberIsNull()
    {
        ContactMessage? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<ContactMessage>()))
            .Callback<ContactMessage>(m => captured = m)
            .Returns(Task.CompletedTask);

        await sut.SubmitAsync(MakeViewModel(phone: null));

        Assert.That(captured!.PhoneNumber, Is.Null);
    }

    [Test]
    public async Task SubmitAsync_AddAsyncCalledOnce()
    {
        await sut.SubmitAsync(MakeViewModel());

        repo.Verify(r => r.AddAsync(It.IsAny<ContactMessage>()), Times.Once);
    }

    [Test]
    public async Task SubmitAsync_SaveChangesCalledAfterAdd()
    {
        var callOrder = new List<string>();
        repo.Setup(r => r.AddAsync(It.IsAny<ContactMessage>()))
            .Callback(() => callOrder.Add("Add"))
            .Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync())
            .Callback(() => callOrder.Add("Save"))
            .Returns(Task.CompletedTask);

        await sut.SubmitAsync(MakeViewModel());

        Assert.That(callOrder, Is.EqualTo(new[] { "Add", "Save" }));
    }

    [Test]
    public void SubmitAsync_WhenRepositoryThrows_ExceptionPropagates()
    {
        repo.Setup(r => r.AddAsync(It.IsAny<ContactMessage>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.SubmitAsync(MakeViewModel()));
    }
}