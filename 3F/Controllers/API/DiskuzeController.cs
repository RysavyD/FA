using System;
using System.Linq;
using System.Web.Http;
using _3F.Log;
using _3F.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Model.Repositories.Interface;
using _3F.Model.Service;
using _3F.Web.Controllers.API.Model;
using _3F.Web.Extensions;
using _3F.Web.Models;
using _3F.Web.Models.Discussion;

namespace _3F.Web.Controllers.API
{
    public class DiskuzeController : BaseApiController
    {
        private readonly ILogger logger;
        private readonly IActivityListService activityListService;
        private readonly IDiscussionRepository _discussionRepository;

        public DiskuzeController(IRepository repository, ILogger logger, IActivityListService activityListService, IDiscussionRepository discussionRepository)
            : base(repository)
        {
            this.logger = logger;
            this.activityListService = activityListService;
            _discussionRepository = discussionRepository;
        }

        [HttpGet]
        public IHttpActionResult Get(int id = 1, int page = 1, int pagesize = 10)
        {
            if (!User.Identity.IsAuthenticated)
                page = 1; // nepřihlášený má přístup pouze na první stránku

            var result = new ApiResultList<DiscussionItemViewModel>("discussionItemTemplate");
            var discussionItems = _discussionRepository.GetDiscussionItems(id, page, pagesize);
            result.Items = discussionItems.Item2.Select(d => new DiscussionItemViewModel()
            {
                Text = d.Text,
                DateTime = d.DateTime.ToDayDateTimeString(),
                Author = new User(d.Author),
            })
                .ToList();
            result.TotalItems = (User.Identity.IsAuthenticated) ? discussionItems.Item1 : 1;
            result.Page = page;

            return Ok(result);
        }

        [HttpPost, ApiAuthorize]
        public string Post(NewDiscussionMessage newDiscussionMessage)
        {
            if (string.IsNullOrWhiteSpace(newDiscussionMessage.message))
                return "Text nesmí být prázdný";

            try
            {
                var discussion = repository.One<Discussion>(newDiscussionMessage.id);
                discussion.LastUpdateDate = Info.CentralEuropeNow;
                repository.Add(new DiscussionItem()
                    {
                        DateTime = discussion.LastUpdateDate.Value,
                        Id_Discussion = newDiscussionMessage.id,
                        Text = newDiscussionMessage.message.ToHtml(),
                        Id_Author = GetUserId,
                    });

                if (discussion.IsPublic)
                { 
                    logger.LogInfo(string.Format("Uživatel {1} nový příspěvěk u diskuze {0} API", newDiscussionMessage.id, User.Identity.Name), "DiskuzeController.Post");
                }

                activityListService.AddActivity(ActivityCreator.Create(GetUser, discussion, ActivityType.CommentDiscussion));

                return string.Empty;
            }
            catch(Exception ex)
            {
                logger.LogException(ex, "DiskuzeController.Post");
                return ex.Message;
            }
        }

        [HttpGet, Compress]
        public IHttpActionResult List(int page = 1, int pagesize = 10)
        {
            var result = new ApiResultList<ApiDiscussion>()
            {
                Items = repository.Where<Discussion>(d => d.IsAlone)
                    .OrderBy(d => d.Id)
                    .Skip((page - 1)* pagesize)
                    .Take(pagesize)
                    .Select(d => new ApiDiscussion()
                    {
                        Author = new ApiUser()
                        {
                            Name = d.AspNetUsers.UserName,
                            HtmlName = d.AspNetUsers.HtmlName,
                            Photo = d.AspNetUsers.ProfilePhoto,
                        },
                        HtmlName = d.HtmlName,
                        ItemsCount = d.DiscussionItem.Count,
                        Name = d.Name,
                        Perex = d.Perex,
                        CreateDate = d.CreateDate,
                        LastUpdateDate = d.LastUpdateDate,
                    })
                    .ToList(),
                Page = page,
                PageSize = pagesize,
                TotalItems = repository.Where<Discussion>(d => d.IsAlone).Count(),
            };

            return Ok(result);
        }

        [HttpGet, ApiAuthorize, Compress]
        public IHttpActionResult Detail(string id, int page = 1, int pagesize = 10)
        {
            var discussion = repository.OneByHtml<Discussion>(id)
                .DiscussionItem
                .OrderBy(di => di.DateTime)
                .Skip((page - 1)*pagesize)
                .Take(pagesize);
            var result = new ApiResultList<ApiDiscussionItem>()
            {
                Items = discussion
                    .Select(di => new ApiDiscussionItem()
                    {
                        Author = new ApiUser()
                        {
                            Name = di.AspNetUsers.UserName,
                            HtmlName = di.AspNetUsers.HtmlName,
                            Photo = di.AspNetUsers.ProfilePhoto,
                        },
                        DateTime = di.DateTime,
                        Text = di.Text,
                    })
                    .ToList(),
                Page = page,
                PageSize = pagesize,
                TotalItems = repository.OneByHtml<Discussion>(id).DiscussionItem.Count()
            };

            return Ok(result);
        }
    }
}
