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

        public AdminMessageService(IContactMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public async Task<AdminMessagesViewModel> GetMessagesAsync(string filter)
        {
            var messages = await messageRepository.GetAllAsync(filter);
            int unreadCount = await messageRepository.GetUnreadCountAsync();
            int totalCount = await messageRepository.GetTotalCountAsync();

            return new AdminMessagesViewModel
            {
                UnreadCount = unreadCount,
                TotalCount = totalCount,
                Messages = messages.Select(MapToRow)
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
        }

        public async Task MarkUnreadAsync(int id)
        {
            var message = await GetTrackedOrThrowAsync(id);
            message.IsRead = false;
            await messageRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var message = await GetTrackedOrThrowAsync(id);
            await messageRepository.DeleteAsync(message);
        }


        // helper methods
        private async Task<ContactMessage> GetTrackedOrThrowAsync(int id)
            => await messageRepository.GetByIdTrackedAsync(id)
               ?? throw new InvalidOperationException($"Message {id} not found.");

        private static AdminMessageRowViewModel MapToRow(ContactMessage m)
        {
            return new AdminMessageRowViewModel
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
}
