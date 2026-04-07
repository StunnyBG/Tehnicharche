using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tehnicharche.ViewModels;

namespace Tehnicharche.Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly int[] ErrorCodesWithPages = { 400, 403, 404, 405, 429 };

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            if (statusCode != null)
            {
                model.StatusCode = statusCode;

                if (ErrorCodesWithPages.Contains(statusCode.Value))
                    return View("Error" + statusCode.ToString());
                else if (statusCode == 500)
                    return View("Error500", model);
            }

            return View("Error", model);
        }
    }
}