using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using _3F.Model.Accounting;

namespace _3F.Web.Controllers
{
    [Authorize(Roles = Definitions.Strings.Administrator)]
    public class AccountingController : Controller
    {
        private readonly IAccounting _accounting;

        public AccountingController(IAccounting accounting)
        {
            _accounting = accounting;
        }

        public async Task<JsonResult> GetCosts(int id)
        {
            var costs = await _accounting.GetCosts(id);
            return Json(new {suma = costs.Sum(x => x.Amount) }, JsonRequestBehavior.AllowGet);
        }
    }
}