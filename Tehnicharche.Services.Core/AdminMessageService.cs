using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels.Admin;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Services.Core
{
    public class AdminMessageService : IAdminMessageService
    {
        private readonly IContactMessageRepository messageRepository;
        private readonly ILogger<AdminMessageService> logger;

        public AdminMessageService(
            IContactMessageRepository messageRepository,
            ILogger<AdminMessageService> logger)
        {
            this.messageRepository = messageRepository;
            this.logger = logger;
        }

        public async Task<AdminMessagesViewModel> GetMessagesAsync(string filter, int page)
        {
            page = page <= 0 ? 1 : page;

            var (items, filteredTotal) = await messageRepository.GetAllAsync(filter, page, AdminPageSize);

            int unreadCount = await messageRepository.GetUnreadCountAsync();
            int totalCount = await messageRepository.GetTotalCountAsync();

            int totalPages = (int)Math.Ceiling((double)filteredTotal / AdminPageSize);
            if (totalPages < 1) totalPages = 1;

            return new AdminMessagesViewModel
            {
                UnreadCount = unreadCount,
                TotalCount = totalCount,
                Page = page,
                TotalPages = totalPages,
                Messages = items.Select(MapToRow)
            };
        }

        public async Task<AdminMessageRowViewModel> GetByIdAsync(int id)
        {
            var message = await messageRepository.GetByIdTrackedAsync(id)
                ?? throw new InvalidOperationException($"Message {id} not found.");

            if (!message.IsRead)
            {
                message.IsRead = true;
                await messageRepository.SaveChangesAsync();
                logger.LogInformation("Message {MessageId} marked as read on first view.", id);
            }

            return MapToRow(message);
        }

        public async Task<IEnumerable<AdminMessageRowViewModel>> GetRecentAsync(int count)
        {
            var messages = await messageRepository.GetRecentAsync(count);
            return messages.Select(MapToRow);
        }

        public async Task<int> GetUnreadCountAsync()
            => await messageRepository.GetUnreadCountAsync();

        public async Task MarkReadAsync(int id)
        {
            var message = await GetTrackedOrThrowAsync(id);
            message.IsRead = true;
            await messageRepository.SaveChangesAsync();
            logger.LogInformation("Message {MessageId} marked as read by admin.", id);
        }

        public async Task MarkUnreadAsync(int id)
        {
            var message = await GetTrackedOrThrowAsync(id);
            message.IsRead = false;
            await messageRepository.SaveChangesAsync();
            logger.LogInformation("Message {MessageId} marked as unread by admin.", id);
        }

        public async Task DeleteAsync(int id)
        {
            var message = await GetTrackedOrThrowAsync(id);
            await messageRepository.DeleteAsync(message);
            logger.LogInformation("Message {MessageId} permanently deleted by admin.", id);
        }

        // helpers 
        private async Task<ContactMessage> GetTrackedOrThrowAsync(int id)
            => await messageRepository.GetByIdTrackedAsync(id)
               ?? throw new InvalidOperationException($"Message {id} not found.");

        private static AdminMessageRowViewModel MapToRow(ContactMessage m) =>
            new AdminMessageRowViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                Subject = m.Subject,
                Message = m.Message,
                IsRead = m.IsRead,
                SentAt = m.SentAt.ToString(DateFormat)
            };
    }
}