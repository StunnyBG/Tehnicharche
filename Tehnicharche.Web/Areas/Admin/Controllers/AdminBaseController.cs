using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Web.Areas.Admin.Controllers
{
    [Area(AdminArea)]
    [Authorize(Roles = AdminRole)]
    [AutoValidateAntiforgeryToken]
    public abstract class AdminBaseController : Controller { }
}
