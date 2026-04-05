using Microsoft.Extensions.Logging;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Services.Core.Interfaces;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Services.Core
{
    public class ContactService : IContactService
    {
        private readonly IContactMessageRepository messageRepository;
        private readonly ILogger<ContactService> logger;

        public ContactService(
            IContactMessageRepository messageRepository,
            ILogger<ContactService> logger)
        {
            this.messageRepository = messageRepository;
            this.logger = logger;
        }

        public async Task SubmitAsync(ContactFormViewModel model)
        {
            var message = new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Subject = model.Subject,
                Message = model.Message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await messageRepository.AddAsync(message);
            await messageRepository.SaveChangesAsync();

            logger.LogInformation(
                "Contact message received from '{Email}' with subject '{Subject}'.",
                model.Email, model.Subject);
        }
    }
}