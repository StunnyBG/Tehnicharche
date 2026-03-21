using Microsoft.AspNetCore.Mvc;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Web.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        public IActionResult Index(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // in a real app this would send an email or save to DB
            TempData["SuccessMessage"] = "Thank you for reaching out! We'll get back to you as soon as possible.";
            return RedirectToAction(nameof(Index));
        }
    }
}
