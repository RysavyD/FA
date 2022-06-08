using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _3F.BusinessEntities.Diskuze;
using _3F.Model.Repositories.Interface;
using _3F.Model.Service;
using _3F.Model.Service.Model;
using _3F.Web.Models;
using _3F.Web.Models.Discussion;

namespace _3F.Web.Controllers
{
    public class HomeController : BaseController
    {
        private IActivityListService activityListService;
        private IDiscussionRepository discussionRepository;

        public HomeController(IActivityListService activityListService, IDiscussionRepository discussionRepository)
        {
            this.activityListService = activityListService;
            this.discussionRepository = discussionRepository;
        }

        public ActionResult Index()
        {
            var model = CreateModel(EventsType.actual);
            ViewBag.DescriptionIcon = "icon-star";

            return View(model);
        }

        public ActionResult Uplynule()
        {
            var model = CreateModel(EventsType.old);
            ViewBag.DescriptionIcon = "icon-archive";

            return View("Index", model);
        }

        public ActionResult Prihlasene()
        {
            var model = CreateModel(EventsType.login);
            ViewBag.DescriptionIcon = "icon-check";

            return View("Index", model);
        }

        public ActionResult Organizovane()
        {
            var model = CreateModel(EventsType.organised);
            ViewBag.DescriptionIcon = "icon-user-md";

            return View("Index", model);
        }

        public ActionResult Navstivene()
        {
            var model = CreateModel(EventsType.visited);
            ViewBag.DescriptionIcon = "icon-user";

            return View("Index", model);
        }

        [Authorize(Roles = "Administrator" )]
        public ActionResult Smazane()
        {
            var model = CreateModel(EventsType.deleted);
            ViewBag.DescriptionIcon = "icon-trash";

            return View("Index", model);
        }

        public ActionResult Napady()
        {
            var model = CreateModel(EventsType.suggested);
            ViewBag.DescriptionIcon = "icon-lightbulb";

            return View("Index", model);
        }

        [Route("~/Nerozhodnute")]
        public ActionResult Nerozhodnute()
        {
            var model = CreateModel(EventsType.undecided);
            ViewBag.DescriptionIcon = "icon-question";

            return View("Index", model);
        }

        [OutputCache(Duration = 300, VaryByParam = "none")]
        private IEnumerable<ActivityModel> GetActivities()
        {
            return activityListService.GetActivities();
        }

        private IEnumerable<DiscussionMainPageModel> GetDiscussion()
        {
            //return repository
            //    .Where<Discussion>(d => d.IsAlone)
            //    .OrderByDescending(d => d.LastUpdateDate)
            //    .Take(3)
            //    .Select(d => new DiscussionMainPageModel()
            //    {
            //        Name = d.Name,
            //        HtmlName = d.HtmlName
            //    });

            var discussions = discussionRepository.GetLastDiscsuDiscussions();
            return discussions.Select(ConvertToDiscussionViewModel);
        }

        private DiscussionMainPageModel ConvertToDiscussionViewModel(LastDiscussion lastDiscussion)
        {
            DiscussionMainPageModel result;

            if (!string.IsNullOrEmpty(lastDiscussion.EventName))
                result = new DiscussionMainPageModel()
                {
                    Name = lastDiscussion.EventName,
                    HtmlName = Url.Content("~/Akce/Detail/" + lastDiscussion.EventHtml)
                };
            else if (!string.IsNullOrEmpty(lastDiscussion.SummaryName))
                result = new DiscussionMainPageModel()
                {
                    Name = lastDiscussion.SummaryName,
                    HtmlName = Url.Content("~/Zapis/Detail/" + lastDiscussion.SummaryHtml)
                };
            else if (!string.IsNullOrEmpty(lastDiscussion.PhotoId))
            {
                result = new DiscussionMainPageModel()
                {
                    Name = lastDiscussion.PhotoEventName,
                    HtmlName = Url.Content("~/Fotky/Album/" + lastDiscussion.PhotoId)
                };
            }
            else
            {
                result = new DiscussionMainPageModel()
                {
                    Name = lastDiscussion.DiscussionName,
                    HtmlName = Url.Content("~/Diskuze/Detail/" + lastDiscussion.DiscussionHtml),
                };
            }

            return result;
        }

        private MainPageModel CreateModel(EventsType eventsType)
        {
            return new MainPageModel()
            {
                EventType = eventsType,
                Activities = GetActivities(),
                Discussion = GetDiscussion(),
            };
        }

        [Route("~/Kalendar")]
        public ActionResult Calendar()
        {
            return View(new EmptyBaseViewModel("Kalendář")
            {
                Icon = "icon-calendar",
            });
        }
    }
}