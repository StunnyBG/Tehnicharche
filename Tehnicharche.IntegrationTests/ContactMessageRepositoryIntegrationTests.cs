using NUnit.Framework;
using Tehnicharche.Data;
using Tehnicharche.Data.Repositories;
using Tehnicharche.Tests.Integration.Helpers;

namespace Tehnicharche.Tests.Integration
{
    public class ContactMessageRepositoryIntegrationTests
    {
        private TehnicharcheDbContext context = null!;
        private ContactMessageRepository sut = null!;

        [SetUp]
        public void SetUp()
        {
            context = DbContextFactory.Create();
            sut = new ContactMessageRepository(context);
        }

        [TearDown]
        public void TearDown() => context.Dispose();

        // GetAllAsync

        [Test]
        public async Task GetAllAsync_FilterAll_ReturnsAllMessages()
        {
            context.ContactMessages.AddRange(
                SeedHelpers.MakeContactMessage(1, isRead: true),
                SeedHelpers.MakeContactMessage(2, isRead: false),
                SeedHelpers.MakeContactMessage(3, isRead: false)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetAllAsync("all", 1, 10);

            Assert.That(total, Is.EqualTo(3));
            Assert.That(items.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllAsync_FilterUnread_ReturnsOnlyUnread()
        {
            context.ContactMessages.AddRange(
                SeedHelpers.MakeContactMessage(1, isRead: true),
                SeedHelpers.MakeContactMessage(2, isRead: false),
                SeedHelpers.MakeContactMessage(3, isRead: false)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetAllAsync("unread", 1, 10);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(m => !m.IsRead), Is.True);
        }

        [Test]
        public async Task GetAllAsync_FilterRead_ReturnsOnlyRead()
        {
            context.ContactMessages.AddRange(
                SeedHelpers.MakeContactMessage(1, isRead: true),
                SeedHelpers.MakeContactMessage(2, isRead: true),
                SeedHelpers.MakeContactMessage(3, isRead: false)
            );
            await context.SaveChangesAsync();

            var (items, total) = await sut.GetAllAsync("read", 1, 10);

            Assert.That(total, Is.EqualTo(2));
            Assert.That(items.All(m => m.IsRead), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Pagination_ReturnsCorrectSlice()
        {
            for (int i = 1; i <= 6; i++)
                context.ContactMessages.Add(SeedHelpers.MakeContactMessage(i));
            await context.SaveChangesAsync();

            var (page1, total) = await sut.GetAllAsync("all", 1, 2);
            var (page2, _) = await sut.GetAllAsync("all", 2, 2);
            var (page3, _) = await sut.GetAllAsync("all", 3, 2);

            Assert.That(total, Is.EqualTo(6));
            Assert.That(page1.Count(), Is.EqualTo(2));
            Assert.That(page2.Count(), Is.EqualTo(2));
            Assert.That(page3.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_OrderedByMostRecentFirst()
        {
            var old = SeedHelpers.MakeContactMessage(1);
            old.SentAt = DateTime.UtcNow.AddDays(-2);

            var recent = SeedHelpers.MakeContactMessage(2);
            recent.SentAt = DateTime.UtcNow;

            var mid = SeedHelpers.MakeContactMessage(3);
            mid.SentAt = DateTime.UtcNow.AddDays(-1);

            context.ContactMessages.AddRange(old, recent, mid);
            await context.SaveChangesAsync();

            var (items, _) = await sut.GetAllAsync("all", 1, 10);
            var ordered = items.ToList();

            Assert.That(ordered[0].Id, Is.EqualTo(2)); // most recent
            Assert.That(ordered[2].Id, Is.EqualTo(1)); // oldest
        }

        // GetByIdAsync / GetByIdTrackedAsync

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsMessage()
        {
            context.ContactMessages.Add(SeedHelpers.MakeContactMessage(1, subject: "Hello"));
            await context.SaveChangesAsync();

            var result = await sut.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Subject, Is.EqualTo("Hello"));
        }

        [Test]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            var result = await sut.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdTrackedAsync_ReturnsTrackedEntity_ChangesArePersisted()
        {
            context.ContactMessages.Add(SeedHelpers.MakeContactMessage(1, isRead: false));
            await context.SaveChangesAsync();

            var tracked = await sut.GetByIdTrackedAsync(1);
            tracked!.IsRead = true;
            await sut.SaveChangesAsync();

            var stored = await sut.GetByIdAsync(1);
            Assert.That(stored!.IsRead, Is.True);
        }

        // GetRecentAsync

        [Test]
        public async Task GetRecentAsync_ReturnsAtMostNMessages()
        {
            for (int i = 1; i <= 8; i++)
                context.ContactMessages.Add(SeedHelpers.MakeContactMessage(i));
            await context.SaveChangesAsync();

            var result = await sut.GetRecentAsync(5);

            Assert.That(result.Count(), Is.EqualTo(5));
        }

        [Test]
        public async Task GetRecentAsync_ReturnsNewestFirst()
        {
            var old = SeedHelpers.MakeContactMessage(1);
            old.SentAt = DateTime.UtcNow.AddDays(-5);

            var newer = SeedHelpers.MakeContactMessage(2);
            newer.SentAt = DateTime.UtcNow;

            context.ContactMessages.AddRange(old, newer);
            await context.SaveChangesAsync();

            var result = (await sut.GetRecentAsync(2)).ToList();

            Assert.That(result[0].Id, Is.EqualTo(2));
        }

        // GetUnreadCountAsync / GetTotalCountAsync

        [Test]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
            context.ContactMessages.AddRange(
                SeedHelpers.MakeContactMessage(1, isRead: false),
                SeedHelpers.MakeContactMessage(2, isRead: false),
                SeedHelpers.MakeContactMessage(3, isRead: true)
            );
            await context.SaveChangesAsync();

            var count = await sut.GetUnreadCountAsync();

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetTotalCountAsync_ReturnsAllMessages()
        {
            context.ContactMessages.AddRange(
                SeedHelpers.MakeContactMessage(1, isRead: true),
                SeedHelpers.MakeContactMessage(2, isRead: false)
            );
            await context.SaveChangesAsync();

            var total = await sut.GetTotalCountAsync();

            Assert.That(total, Is.EqualTo(2));
        }

        // AddAsync / SaveChangesAsync

        [Test]
        public async Task AddAsync_MessageIsPersisted()
        {
            var message = SeedHelpers.MakeContactMessage(1, subject: "Bug Report");

            await sut.AddAsync(message);
            await sut.SaveChangesAsync();

            var stored = await sut.GetByIdAsync(1);
            Assert.That(stored, Is.Not.Null);
            Assert.That(stored!.Subject, Is.EqualTo("Bug Report"));
        }

        // DeleteAsync

        [Test]
        public async Task DeleteAsync_RemovesMessageFromDatabase()
        {
            context.ContactMessages.Add(SeedHelpers.MakeContactMessage(1));
            await context.SaveChangesAsync();

            var message = await sut.GetByIdTrackedAsync(1);
            await sut.DeleteAsync(message!);

            var stored = await sut.GetByIdAsync(1);
            Assert.That(stored, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_UnreadCount_DecreasesAfterDeletion()
        {
            context.ContactMessages.AddRange(
                SeedHelpers.MakeContactMessage(1, isRead: false),
                SeedHelpers.MakeContactMessage(2, isRead: false)
            );
            await context.SaveChangesAsync();

            var msg = await sut.GetByIdTrackedAsync(1);
            await sut.DeleteAsync(msg!);

            Assert.That(await sut.GetUnreadCountAsync(), Is.EqualTo(1));
        }
    }
}