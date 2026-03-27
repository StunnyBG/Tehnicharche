using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Data;
using Tehnicharche.Data.Models;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly TehnicharcheDbContext context;

        public ContactController(TehnicharcheDbContext context)
        {
            this.context = context;
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
            {
                return View(model);
            }

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

            await context.ContactMessages.AddAsync(message);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you for reaching out! We'll get back to you as soon as possible.";
            return RedirectToAction(nameof(Index));
        }
    }
}
