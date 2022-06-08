using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using _3F.Log;
using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Controllers.API.Model;
using _3F.Web.Extensions;
using _3F.Web.Models;
using _3F.Web.Utils;
using _3F.Model.Extensions;
using _3F.Model.Repositories;
using _3F.Model.Repositories.Models;

namespace _3F.Web.Controllers.API
{
    public class EventController : BaseApiController
    {
        private ILogToEvent logToEvent;
        private ILogger logger;
        private IEventRepository eventRepository;

        public EventController(IRepository repository, ILogger logger, ILogToEvent logToEvent, IEventRepository eventRepository)
            : base(repository)
        {
            this.logger = logger;
            this.logToEvent = logToEvent;
            this.eventRepository = eventRepository;
        }

        [HttpPost, ApiAuthorize]
        public string Login(EventUserLog eventUserLog)
        {
            logger.LogInfo(string.Format("Uživatel {0} klikl u akce {1} na {2}, referrer {3}", User.Identity.Name, eventUserLog.EventName, eventUserLog.Status, Request.Headers.Referrer), "EventController.Login");

            EventLoginEnum eventLogin;
            if (eventUserLog.Status == "Prijdu")
                eventLogin = EventLoginEnum.Prijdu;
            else if (eventUserLog.Status == "Mozna")
                eventLogin = EventLoginEnum.Mozna;
            else if (eventUserLog.Status == "Neprijdu")
                eventLogin = EventLoginEnum.Neprijdu;
            else
            {
                return string.Empty;
            }

            try
            {
                var eventEntity = repository.OneByHtml<Event>(eventUserLog.EventName);
                var user = repository.One<AspNetUsers>(u => u.UserName == User.Identity.Name);
                var result = logToEvent.LogToEvent(eventEntity, user, eventLogin, eventEntity.State != EventStateEnum.InWork);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogException(ex, "EventController.Login");
                return ex.Message;
            }
        }

        [HttpGet, Compress, Identity]
        public IHttpActionResult Events(EventsType type, int page, string filterName = "", int count = 10)
        {
            bool showAllPages = (User.Identity.IsAuthenticated || (type == EventsType.old || type == EventsType.actual || type == EventsType.category || type == EventsType.suggested));
            var userId = GetUserId;
            if (!showAllPages)
                page = 1; // nepřihlášený má přístup pouze na první stránku, pokud není dáno jinak

            var result = new ApiResultList<MainPageEventModel>();
            var events = repository.All<Event>();

            switch (type)
            {
                case EventsType.actual:
                {
                    var eventEntities = eventRepository.GetActualEvents(userId, page, count);
                    result.TotalItems = eventEntities.Item1;
                    result.Items.AddRange(ModifyEvents(eventEntities.Item2, true));
                    break;
                }
                case EventsType.deleted:
                {
                    if (User.IsInRole(Definitions.Strings.Administrator))
                        events = events
                            .Where(ev => ev.State == EventStateEnum.Deleted)
                            .Where(ev => ev.EventType != EventTypeEnum.Soukroma)
                            .OrderByDescending(ev => ev.StartDateTime);
                    else
                        return Ok(new List<MainPageEventModel>());
                    break;
                }
                case EventsType.login:
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var id_User = GetUserId;
                        var loginStatuses = new []
                        {
                            EventLoginEnum.Prijdu,
                            EventLoginEnum.Mozna,
                            EventLoginEnum.Nahradnik,
                            EventLoginEnum.NepotvrzenaRezervace,
                            EventLoginEnum.Rezervace,
                            EventLoginEnum.RezervacePropadla
                        };

                        events = events
                            .Where(e => e.State == EventStateEnum.Active && e.StopDateTime > Info.CentralEuropeNow)
                            .Where(
                                e =>
                                    e.EventParticipant.Any(
                                        p => p.Id_User == id_User && loginStatuses.Contains(p.EventLoginStatus)))
                            .OrderBy(e => e.StartDateTime);
                    }
                    else
                        return Ok(new List<MainPageEventModel>());

                    break;
                }
                case EventsType.old:
                {
                        var eventEntities = eventRepository.GetOldEvents(userId, page, count);
                        result.TotalItems = eventEntities.Item1;
                        result.Items.AddRange(ModifyEvents(eventEntities.Item2));
                        break;
                }
                case EventsType.organised:
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var id_User = GetUserId;
                        events = events
                            .Where(e => e.EventOrganisator.Any(o => o.Id_User == id_User))
                            .OrderByDescending(e => e.StartDateTime);
                    }
                    else
                        return Ok(new List<MainPageEventModel>());

                    break;
                }
                case EventsType.visited:
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var id_User = GetUserId;
                        events = events
                            .Where(e => e.State == EventStateEnum.Active && e.StopDateTime < Info.CentralEuropeNow)
                            .Where(
                                e =>
                                    e.EventParticipant.Any(
                                        p => p.Id_User == id_User && p.EventLoginStatus == EventLoginEnum.Prijdu))
                            .OrderByDescending(e => e.StartDateTime);
                    }
                    else
                        return Ok(new List<MainPageEventModel>());

                    break;
                }
                case EventsType.undecided:
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var id_User = GetUserId;
                        events = events
                            .Where(ev => ev.State == EventStateEnum.Active && ev.StopDateTime > Info.CentralEuropeNow)
                            .Where(
                                ev =>
                                    ev.EventType != EventTypeEnum.Soukroma ||
                                    ev.EventInvitation.Any(evi => evi.Id_User == userId))
                            .Where(ev => !ev.EventParticipant.Any(ep => ep.Id_User == id_User))
                            .OrderBy(e => e.StartDateTime);
                    }
                    else
                        return Ok(new List<MainPageEventModel>());

                    break;
                }
                case EventsType.category:
                {
                    var categoryId = repository.OneByHtml<EventCategory>(filterName).Id;
                    events = events
                        .Where(ev => ev.State == EventStateEnum.Active)
                        .Where(
                            ev =>
                                ev.EventType != EventTypeEnum.Soukroma ||
                                ev.EventInvitation.Any(evi => evi.Id_User == userId))
                        .Where(ev => ev.EventCategories.Any(ec => ec.Id == categoryId))
                        .OrderByDescending(e => e.StartDateTime);
                    break;
                }
                case EventsType.suggested:
                    {
                        var eventEntities = eventRepository.GetActualEvents(userId, page, count);
                        var suggestedEntities = eventEntities.Item2.Where(ev => ev.IsSuggested).ToArray();
                        result.TotalItems = suggestedEntities.Count();
                        result.Items.AddRange(ModifyEvents(suggestedEntities, true));
                        break;
                    }
                default:
                    return Ok(new List<MainPageEventModel>());
            }

            int? id_user = null;
            if (User.Identity.IsAuthenticated)
                id_user = GetUserId;

            if (type != EventsType.old && type != EventsType.actual && type != EventsType.suggested)
            {
                result.Items.AddRange(
                    events.Skip((page - 1)*count).Take(count).ToArray().Select(di => di.ToMainPageViewModel(id_user)));
                result.TotalItems = (showAllPages) ? events.Count() : 1;
            }
            result.Page = page;
            result.ShowPagination = showAllPages;

            return Ok(result);
        }

        [HttpGet, Compress, Identity]
        public int UndecidedEvents()
        {
            return eventRepository.UndecidedEventsCount(GetUserId);
        }

        private IEnumerable<MainPageEventModel> ModifyEvents(IEnumerable<EventListModel> events, bool showButtons = false)
        {
            var isUser = User.Identity.IsAuthenticated;

            // TODO: prepsat na select !!
            foreach (var eventEntity in events)
            {
                var newEntity = new MainPageEventModel(eventEntity);
                newEntity.YesColor = (eventEntity.ParticipantStatus == 1) ? "#49bf67" : "#808080";
                newEntity.MayBeColor = (eventEntity.ParticipantStatus == 2) ? "#e48a07" : "#808080";
                newEntity.NoColor = (eventEntity.ParticipantStatus == 3) ? "#f34541" : "#808080";
                newEntity.HasDiscussionItem = eventEntity.LastDiscussionItemDate.HasValue;
                newEntity.LastDiscussionItem = (eventEntity.LastDiscussionItemDate.HasValue)
                    ? eventEntity.LastDiscussionItemDate.Value.ToDayDateTimeString()
                    : string.Empty;
                newEntity.Status = (eventEntity.ParticipantStatus.HasValue) 
                    ? ((EventLoginEnum)eventEntity.ParticipantStatus).GetDescription() 
                    : (isUser) ? EventLoginEnum.Nevyjadreno.GetDescription() : string.Empty;

                newEntity.ShowButtons = showButtons && isUser;

                yield return newEntity;
            }
        }

        [HttpGet, Compress, ApiAuthorize]
        public IHttpActionResult Detail(string id)
        {
            var entity = repository.OneByHtml<Event>(id);
            var model = new EventDetail()
            {
                AccountSymbol = entity.AccountSymbol,
                Capacity = entity.Capacity,
                Contact = entity.Contact,
                Description = entity.Description,
                HtmlName = entity.HtmlName,
                Id = entity.Id,
                Id_Discussion = entity.Id_Discussion,
                LastPaidDateTime = entity.LastPaidDateTime,
                LastSignINDateTime = entity.LastSignINDateTime,
                Link = entity.Link,
                MayBeLogOn = entity.MayBeLogOn,
                MeetDateTime = entity.MeetDateTime.Value,
                MeetPlace = entity.MeetPlace,
                MinimumParticipants = entity.MinimumParticipants,
                Name = entity.Name,
                Perex = entity.Perex,
                Photo = entity.Photo,
                Place = entity.Place,
                Price = entity.Price,
                StartDateTime = entity.StartDateTime,
                StopDateTime = entity.StopDateTime,
                State = (int)entity.State,
                Participants = entity.EventParticipant.Select(ep => new ApiParticipant()
                {
                    HtmlName = ep.AspNetUsers.HtmlName,
                    Name = ep.AspNetUsers.UserName,
                    Photo = ep.AspNetUsers.ProfilePhoto,
                    Status = (int)ep.EventLoginStatus,
                    IsExtern = ep.IsExternal,
                }),
                Organisators = entity.EventOrganisator.Select(ep => new ApiUser()
                {
                    HtmlName = ep.AspNetUsers.HtmlName,
                    Name = ep.AspNetUsers.UserName,
                    Photo = ep.AspNetUsers.ProfilePhoto,
                }),
            };

            return Json(model);
        }

        [HttpGet, Compress]
        public IHttpActionResult CalendarEvents(int year, int month)
        {
            var events = eventRepository.GetEventsFromMonth(year, month);
            var model = events.Select(ev => new
            {
                ev.Name,
                ev.HtmlName,
                ev.Start,
                End = ev.Stop,
                ev.Perex,
            });

            return Ok(model);
        }
    }
}
