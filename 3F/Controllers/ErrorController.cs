using System.Web.Mvc;
using _3F.Web.Models;

namespace _3F.Web.Controllers
{
    [Authorize]
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult Index()
        {
            return View(new EmptyBaseViewModel("Chyba"));
        }
    }
}