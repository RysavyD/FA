using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Model;

namespace _3F.Web.Controllers
{
    [Authorize]
    public class TuristickeVizitkyController : BaseCollectorController<TouristCard, TouristCardOwner>
    {
        public TuristickeVizitkyController()
        {
            Icon = "icon-credit-card";
            BackgroundColor = "brown-background";
            ControllerName = "Turistické vizitky";
            Import = new TouristCardDownload(repository, logger);
        }
    }
}