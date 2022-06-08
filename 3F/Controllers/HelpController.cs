using System.Web.Mvc;
using _3F.Model.Repositories;
using _3F.Web.Models;

namespace _3F.Web.Controllers
{
    public class HelpController : BaseController
    {
        private readonly IHelpRepository _repository;

        public HelpController(IHelpRepository repository)
        {
            _repository = repository;
        }

        [Authorize]
        public ActionResult Index()
        {
            var help = _repository.GetAll();
            var model = new EnumerableBaseViewModel<BusinessEntities.Help>(help)
            {
                Title = "Nápověda",
                Icon = "icon-question",
                BackgroundColor = "sea-blue-background",
            };

            return View(model);
        }
    }
}