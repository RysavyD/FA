using System.Web.Mvc;

namespace _3F.Web.Controllers
{
    public class BadmintonController : BaseController
    {
        public ActionResult Index()
        {
            return TextDetail("BadmintonTurnaje");
        }
    }
}