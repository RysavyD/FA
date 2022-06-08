using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _3F.Model.Model;
using _3F.Model.Service;
using _3F.Model.Service.Model;
using _3F.Web.Models;
using _3F.Web.Models.Discussion;

namespace _3F.Web.Controllers
{
    public class KategorieController : BaseController
    {
        private IActivityListService activityListService;

        public KategorieController(IActivityListService activityListService)
        {
            BackgroundColor = "sea-background";
            Icon = "icon-question";
            this.activityListService = activityListService;
        }

        [Route("Kategorie/{id}")]
        public ActionResult Index(string id)
        {
            var model = new MainPageModel()
            {
                EventType = EventsType.category,
                Activities = GetActivities(),
                CategoryHtmlName = id,
                CategoryName = repository.OneByHtml<EventCategory>(id).Name,
                Discussion = GetDiscussion(),
            };

            ViewBag.DescriptionIcon = "icon-filter";

            return View("~/Views/Home/Index.cshtml", model);
        }

        [OutputCache(Duration = 300, VaryByParam = "none")]
        private IEnumerable<ActivityModel> GetActivities()
        {
            return activityListService.GetActivities();
        }

        private IEnumerable<DiscussionMainPageModel> GetDiscussion()
        {
            return repository
                .Where<Discussion>(d => d.IsAlone)
                .OrderByDescending(d => d.LastUpdateDate)
                .Take(3)
                .Select(d => new DiscussionMainPageModel()
                {
                    Name = d.Name,
                    HtmlName = d.HtmlName
                });
        }
    }
}