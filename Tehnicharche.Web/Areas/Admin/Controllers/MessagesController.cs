using Microsoft.AspNetCore.Mvc;
using Tehnicharche.Services.Core.Interfaces;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    public class MessagesController : AdminBaseController
    {
        private readonly IAdminMessageService messageService;
        private readonly ILogger<MessagesController> logger;

        public MessagesController(
            IAdminMessageService messageService,
            ILogger<MessagesController> logger)
        {
            this.messageService = messageService;
            this.logger = logger;
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
            try
            {
                await messageService.MarkReadAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to mark message {MessageId} as read.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> MarkUnread(int id, string filter = "all")
        {
            try
            {
                await messageService.MarkUnreadAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to mark message {MessageId} as unread.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string filter = "all")
        {
            try
            {
                await messageService.DeleteAsync(id);
                TempData["AdminSuccess"] = "Message deleted.";
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Failed to delete message {MessageId}.", id);
                TempData["AdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { filter });
        }
    }
}
