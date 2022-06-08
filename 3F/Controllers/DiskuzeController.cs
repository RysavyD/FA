using System;
using System.Linq;
using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Model.Repositories.Interface;
using _3F.Model.Service;
using _3F.Web.Extensions;
using _3F.Web.Models;
using _3F.Web.Models.Discussion;
using _3F.Web.Models.Diskuze;

namespace _3F.Web.Controllers
{
    public class DiskuzeController : BaseController
    {
        private readonly IActivityListService _activityListService;
        private readonly IDiscussionRepository _discussionRepository;

        public DiskuzeController(IActivityListService activityListService, IDiscussionRepository discussionRepository)
        {
            _activityListService = activityListService;
            _discussionRepository = discussionRepository;
            BackgroundColor = "blue-background";
            Icon = "icon-comment-alt";
        }

        public ActionResult Index()
        {
            var list = _discussionRepository.Discussions;
            var model = new EnumerableBaseViewModel<DiscussionViewModel>()
            {
                Title = "Seznam diskuzí",
                Entities = list.Select(d =>
                    new DiscussionViewModel()
                    {
                        Id = d.Id,
                        Name = d.Name,
                        HtmlName = d.HtmlName,
                        Perex = d.Perex,
                        Author = new User(d.Author),
                        ItemsCount = d.ItemsCount,
                        LastUpdateDate = d.LastUpdateDate,
                        CreateDate = d.CreateDate,
                        CreateDateString = d.CreateDate.ToDayDateTimeString(),
                    }),
            };

            return View(model);
        }

        public ActionResult Detail(string id)
        {
            var model = repository.One<Discussion>(d => d.HtmlName == id).ToViewModel();
            model.Title = model.Name;

            if (User.Identity.IsAuthenticated)
                logger.LogInfo(string.Format("Uživatel(ka) {0} navštívil(a) diskuzi {1}", User.Identity.Name, model.Name), "Diskuze.Detail");

            return View(model);
        }

        [Authorize]
        public ActionResult Nova()
        {
            return View(new DiscussionViewModel() { Title = "Založení nové diskuze" });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken, ActionName("Nova")]
        public ActionResult NovaPost(DiscussionViewModel newDiscussion)
        {
            if (ModelState.IsValid)
            {
                var discussion = new Discussion()
                {
                    IsAlone = true,
                    Id_Author = GetUserId,
                    Name = newDiscussion.Name,
                    Perex = newDiscussion.Perex.ToHtml(),
                    CreateDate = Info.CentralEuropeNow,
                    LastUpdateDate = Info.CentralEuropeNow,
                };

                repository.SetHtmlName(discussion, discussion.Name);
                repository.Add(discussion);

                logger.LogInfo(string.Format("Uživatel {0} založil diskuzi {1}", GetUser.UserName, discussion.Name), "Diskuze.Nova");
                _activityListService.AddActivity(ActivityCreator.Create(GetUser, discussion, ActivityType.CreateDiscussion));

                CreateToastrMessage("Diskuze byla úspěšně založena");

                return RedirectToAction("Detail", "Diskuze", new { id = discussion.HtmlName });
            }

            newDiscussion.Title = "Založení nové diskuze";
            return View(newDiscussion);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken, ActionName("Add"), ValidateInput(false)]
        public ActionResult AddPost(int id, string message, int pagesize = 10)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                try
                {
                    var discussion = repository.One<Discussion>(id);
                    discussion.LastUpdateDate = Info.CentralEuropeNow;
                    repository.Add(new DiscussionItem()
                    {
                        DateTime = discussion.LastUpdateDate.Value,
                        Id_Discussion = id,
                        Text = Server.HtmlEncode(message).ToHtml(),
                        Id_Author = GetUserId,
                    });

                    logger.LogInfo(string.Format("Uživatel {1} nový příspěvěk u diskuze {0}", id, User.Identity.Name),
                        "DiskuzeController.Post");

                    if (discussion.IsPublic)
                    {
                        _activityListService.AddActivity(
                            ActivityCreator.Create(GetUser, discussion, ActivityType.CommentDiscussion));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "DiskuzeController.AddPost");
                }
            }

            var result = GetDiscussionItemsPartialViewModel(id, 1, pagesize);

            return PartialView("GetItems", result);
        }

        [HttpPost]
        public ActionResult GetItems(int id, int page = 1, int pagesize = 10)
        {
            if (!User.Identity.IsAuthenticated)
            {
                page = 1;
                pagesize = 10;
            }

            var result = GetDiscussionItemsPartialViewModel(id, page, pagesize);

            return PartialView(result);
        }

        private DiscussionItemsPartialViewModel GetDiscussionItemsPartialViewModel(int id, int page = 1, int pagesize = 10)
        {
            var items = _discussionRepository.GetDiscussionItems(id, page, pagesize);
            int maxPage = User.Identity.IsAuthenticated ? (int) Math.Ceiling(items.Item1 / (decimal) pagesize) : 1;
            maxPage = Math.Max(1, maxPage);

            var result = new DiscussionItemsPartialViewModel()
            {
                Items = items.Item2,
                IdDiscussion = id,
                Page = page,
                StartPage = Math.Max(1, page - 4),
                EndPage = Math.Min(page + 5, maxPage),
                MaxPage = maxPage,
            };

            if (result.EndPage == result.MaxPage)
            {
                result.StartPage = Math.Max(1, result.MaxPage - 9);
            }
            if (result.StartPage == 1)
            {
                result.EndPage = Math.Min(maxPage, 10);
            }

            return result;
        }
    }
}