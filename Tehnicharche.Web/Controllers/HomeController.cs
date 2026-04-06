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

            if (statusCode == 403)
            {
                Response.StatusCode = 403;
                return View("Error403");
            }

            if (statusCode == 404)
            {
                Response.StatusCode = 404;
                return View("Error404");
            }

            if (statusCode == 405)
            {
                Response.StatusCode = 405;
                return View("Error405");
            }

            if (statusCode == 429)
            {
                Response.StatusCode = 429;
                return View("Error429");
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