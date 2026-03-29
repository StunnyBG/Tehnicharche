using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactMessageRepository messageRepository;

        public ContactController(IContactMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

            TempData["SuccessMessage"] = "Thank you for reaching out! We'll get back to you as soon as possible.";
            return RedirectToAction(nameof(Index));
        }
    }
}
