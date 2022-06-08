using _3F.Model.Model;
using _3F.Web.Models;
using System.Linq;
using System.Web.Mvc;

namespace _3F.Web.Controllers
{
    [Authorize]
    public class UzivateleController : BaseController
    {
        public ActionResult Index()
        {
            var users = repository.Where<AspNetUsers>(u => u.LoginType == LoginTypeEnum.Confirmed || u.LoginType == LoginTypeEnum.OldSystemConfirmed)
                .OrderByDescending(u => u.DateLastActivity)
                .ToArray()
                .Select(u => new ActivityUser(u));

            return View(users);
        }
    }
}