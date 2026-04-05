using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == 400)
            {
                Response.StatusCode = 400;
                return View("Error400");
            }

            if (statusCode == 404)
            {
                Response.StatusCode = 404;
                return View("Error404");
            }

            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            Response.StatusCode = 500;
            return View("Error500", model);
        }
    }
}
