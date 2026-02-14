using Microsoft.AspNetCore.Mvc;

namespace Tehnicharche.Web.Controllers
{
    public class ListingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
