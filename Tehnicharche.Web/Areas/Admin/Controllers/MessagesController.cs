using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class MessagesController : AdminBaseController
    {
        private readonly IAdminMessageService messageService;

        public MessagesController(IAdminMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filter = "all", int page = DefaultPage)
        {
            var model = await messageService.GetMessagesAsync(filter, page);
            ViewBag.Filter = filter;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string filter = "all")
        {
            try
            {
                var model = await messageService.GetByIdAsync(id);
                ViewBag.Filter = filter;
                return View(model);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id, string filter = "all")
        {
            await messageService.MarkReadAsync(id);
            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> MarkUnread(int id, string filter = "all")
        {
            await messageService.MarkUnreadAsync(id);
            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string filter = "all")
        {
            await messageService.DeleteAsync(id);
            TempData["AdminSuccess"] = "Message deleted.";
            return RedirectToAction(nameof(Index), new { filter });
        }
    }
}
