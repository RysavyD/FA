using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Accounting;
using _3F.Model.Email;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Model.Repositories;
using _3F.Model.Service;
using _3F.Model.Utils;
using _3F.Web.Controllers.API.Model;
using _3F.Web.Definitions;
using _3F.Web.Extensions;
using _3F.Web.Models;
using _3F.Web.Models.EventModels;
using _3F.Web.Models.Events;
using _3F.Web.Utils;
using _3F.Web.Models.Akce;

namespace _3F.Web.Controllers
{
    public class AkceController : BaseController
    {
        private ILogToEvent logToEvent;
        private IActivityListService activityListService;
        private IEventUtils eventUtils;
        private IAccounting accounting;
        private IMessageService messageService;
        private IEmailSender emailSender;
        private IEventRepository eventRepository;
        private IRajceService rajceService;

        public AkceController(ILogToEvent logToEvent, IActivityListService activityListService, IEventUtils eventUtils, IAccounting accounting, IMessageService messageService, IEmailSender emailSender, IEventRepository eventRepository, IRajceService rajceService)
        {
            this.logToEvent = logToEvent;
            this.activityListService = activityListService;
            this.eventUtils = eventUtils;
            this.accounting = accounting;
            this.messageService = messageService;
            this.emailSender = emailSender;
            this.eventRepository = eventRepository;
            this.rajceService = rajceService;
        }

        #region Choose action type
        [Authorize]
        public ActionResult Nova()
        {
            var eventTypeList = new List<ChooseEventType>();

            eventTypeList.Add(new ChooseEventType()
            {
                Name = "Běžná akce",
                Url = Url.Content("~/Akce/Vytvorit/1"),
                Descriptions = new List<string>()
                {
                    "Jedná se o obyčejnou akci.",
                    "Akce je zdarma nebo je placená na místě."
                }
            });

            if (HasUserAccessToEventType(EventTypeEnum.PlacenaSdruzenim))
            {
                eventTypeList.Add(new ChooseEventType()
                {
                    Name = "Oficiální předplacená akce",
                    Url = Url.Content("~/Akce/Vytvorit/2"),
                    Descriptions = new List<string>()
                    {
                        "Jedná se o akci garantovanou občanským sdružením Společné aktivity o.s..",
                        "Akci bude nutno zaplatit předem."
                    }
                });
            }

            if (HasUserAccessToEventType(EventTypeEnum.OficialniSdruzeni))
            {
                eventTypeList.Add(new ChooseEventType()
                {
                    Name = "Oficiální neplacená akce",
                    Url = Url.Content("~/Akce/Vytvorit/3"),
                    Descriptions = new List<string>()
                    {
                        "Jedná se o akci garantovanou občanským sdružením Společné aktivity o.s..",
                        "Akci se platí na místě nebo je zdarma."
                    }
                });
            }

            if (HasUserAccessToEventType(EventTypeEnum.Soukroma))
            {
                eventTypeList.Add(new ChooseEventType()
                {
                    Name = "Soukromá akce",
                    Url = Url.Content("~/Akce/Vytvorit/4"),
                    Descriptions = new List<string>()
                    {
                        "Jedná se o soukromou akci, na kterou můžete pozvat ostantí uživatele.",
                        "Akce není veřejně viditelná a vidí ji jen pozvaní účastníci.",
                        "Akce je zdarma nebo je placená na místě."
                    }
                });
            }

            if (HasUserAccessToEventType(EventTypeEnum.TipNaAkci))
            {
                eventTypeList.Add(new ChooseEventType()
                {
                    Name = "Nápad na akci",
                    Url = Url.Content("~/Akce/Vytvorit/5"),
                    Descriptions = new List<string>()
                    {
                        "Jedná se o tip na událost, která se bude konat a které se nejspíše nehodláš zúčastnit.",
                        "Případně dlouhodobou událost, kterou nechceš vyhlásit jako běžnou akci.",
                        "Akce je zdarma nebo je placená na místě."
                    }
                });
            }

            var model = new EnumerableBaseViewModel<ChooseEventType>(eventTypeList)
            {
                Title = "Vyberte typ akce, kterou chcete vytvořit",
                Icon = "icon-edit"
            };

            return View(model);
        }

        private bool HasUserAccessToEventType(int eventType)
        {
            return HasUserAccessToEventType((EventTypeEnum)eventType);
        }

        private bool HasUserAccessToEventType(EventTypeEnum eventType)
        {
            switch (eventType)
            {
                case EventTypeEnum.Bezna:
                    return true;
                case EventTypeEnum.OficialniSdruzeni:
                    return User.IsInRole(Strings.CertifiedOrganisator);
                case EventTypeEnum.PlacenaSdruzenim:
                    return User.IsInRole(Strings.CertifiedOrganisator);
                case EventTypeEnum.Soukroma:
                    return User.IsInRole(Strings.CertifiedOrganisator);
                case EventTypeEnum.TipNaAkci:
                        return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Create and edit event
        [Authorize]
        public ActionResult Vytvorit(int id)
        {
            if (HasUserAccessToEventType(id))
            {
                switch (id)
                {
                    case 1:
                        return View(PrepareModel(EventModel.GetEventModel(EventTypeEnum.Bezna)));
                    case 2:
                        return View(PrepareModel(EventModel.GetEventModel(EventTypeEnum.PlacenaSdruzenim)));
                    case 3:
                        return View(PrepareModel(EventModel.GetEventModel(EventTypeEnum.OficialniSdruzeni)));
                    case 4:
                        return View(PrepareModel(EventModel.GetEventModel(EventTypeEnum.Soukroma)));
                    case 5:
                        return View(PrepareModel(EventModel.GetEventModel(EventTypeEnum.TipNaAkci)));
                }
            }

            return View("~/Views/Akce/NoRightEvent.cshtml");
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Vytvorit")]
        public ActionResult VytvoritPost(int id, HttpPostedFileBase file, EventModel model, string submitButton)
        {
            RightsToEventValidation(model);
            BasicValidation(model);

            if (!ModelState.IsValid) // only basic validation (required and range attributes)
                return View(PrepareModel(model));

            try
            {
                if (model.EventType == EventTypeEnum.TipNaAkci)
                {
                    model.MeetPlace = null;
                    model.MeetTime = null;
                    model.OrganisatorNames = GetUserId.ToString();
                }

                // create event
                var entity = model.ToEntity();
                SetOrganisators(entity, model);

                // save as InWork or apply all validation rules
                if (submitButton == "Uložit jako koncept")
                {
                    entity.State = EventStateEnum.InWork;
                }
                else
                {
                    ExtendedValidateEventEntity(entity, null);
                    if (!ModelState.IsValid)
                        return View(PrepareModel(model));
                }

                repository.SetHtmlName(entity, entity.Name);

                // add discussion to event
                var discussionEntity = new Discussion()
                    {Name = model.Name, HtmlName = entity.HtmlName, IsAlone = false};
                repository.Add(discussionEntity);
                entity.Id_Discussion = discussionEntity.Id;

                // set image to event
                SetEventImage(file, entity);

                // set category
                UpdateCategories(model.CategoryIds, entity);

                entity.AccountSymbol = repository.All<Event>().Max(ev => ev.AccountSymbol) + 1;
                repository.Add(entity);

                logger.LogInfo(string.Format("Uživatel {0} vytvořil akci {1}", User.Identity.Name, entity.Name),
                    "Akce.VytvoritPost");

                SetInvitation(entity);
                PublicateEvent(entity);

                return RedirectToAction("Detail", "Akce", new {id = entity.HtmlName});
            }
            catch(Exception ex)
            {
                LogValidationErrors(ex, "Akce.VytvoritPost");
                logger.LogException(ex, "Akce.VytvoritPost");
            }

            return View(PrepareModel(model));
        }


        [Authorize]
        public ActionResult Edit(string id)
        {
            var model = repository.One<Event>(e => e.HtmlName == id).ToViewModel();

            return View(PrepareModel(model));
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Edit")]
        public ActionResult EditPost(HttpPostedFileBase file, EventModel model, string id, string submitButton)
        {
            RightsToEventValidation(model);
            BasicValidation(model);
            string publishEvent = "Zveřejnit akci";
            string publishEventHint = "Zveřejnit tip";

            if (!ModelState.IsValid)
                return View(PrepareModel(model));

            try
            {
                if (model.EventType == EventTypeEnum.TipNaAkci)
                {
                    model.MeetPlace = null;
                    model.MeetTime = null;
                    model.OrganisatorNames = GetUserId.ToString();
                }

                // create temp event
                var entity = model.ToEntity();
                var dbEntity = repository.One<Event>(e => e.HtmlName == id);

                if (dbEntity.State == EventStateEnum.Active
                    || (dbEntity.State == EventStateEnum.InWork && (submitButton == publishEvent || submitButton == publishEventHint)))
                {
                    // for active event apply all validation rules 
                    ExtendedValidateEventEntity(entity, dbEntity);
                    SetOrganisators(dbEntity, model);

                    if (!ModelState.IsValid)
                        return View(PrepareModel(model));
                }

                var oldCapacity = dbEntity.Capacity;
                SetEventImage(file, dbEntity);
                UpdateCategories(model.CategoryIds, dbEntity); // set category
                CopyEventProperties(dbEntity, entity);     // copy properties and save            
                logger.LogInfo(string.Format("Uživatel {0} editoval akci {1}", User.Identity.Name, dbEntity.Name), "Akce.EditPost");

                // show editEvent activity for active non-private action
                if (!dbEntity.IsPrivate && dbEntity.State == EventStateEnum.Active)
                    activityListService.AddActivity(ActivityCreator.Create(GetUser, dbEntity, ActivityType.EditEvent));

                // if capacity was changed recalculate user (move from/to reservation)
                if (oldCapacity != model.Capacity)
                    eventUtils.RecalculateUsers(dbEntity);

                // if event was InWork and user want to publish it
                if (dbEntity.State == EventStateEnum.InWork && (submitButton == publishEvent || submitButton == publishEventHint))
                {
                    dbEntity.State = EventStateEnum.Active;
                    repository.Save();
                    PublicateEvent(dbEntity);
                }

                return RedirectToAction("Detail", "Akce", new { id = dbEntity.HtmlName });
            }
            catch (Exception ex)
            {
                LogValidationErrors(ex, "Akce.EditPost");
                logger.LogException(ex, "Akce.EditPost");
            }
            return View(PrepareModel(model));
        }

        private void RightsToEventValidation(EventModel model)
        {
            if (!HasUserAccessToEventType(model.EventType))
            {
                ModelState.AddModelError("", "Nemáte oprávnění k tomuto typu akce");
            }
        }

        private void BasicValidation(EventModel model)
        {
            if (model.MainCategory == 0)
            {
                ModelState.AddModelError("MainCategory", "Je třeba zvolit kategorii");
            }

            var blockDate = new DateTime(2008, 1, 1);
            if (model.Start < blockDate)
                ModelState.AddModelError("Start", "Čas začátku musí být zadán");

            if (model.Stop < blockDate)
                ModelState.AddModelError("Stop", "Čas konce musí být zadán");

            if (model.EventType != EventTypeEnum.TipNaAkci && model.MeetTime < blockDate)
                ModelState.AddModelError("MeetTime", "Čas srazu musí být zadán");
        }

        private void ExtendedValidateEventEntity(Event newEntity, Event oldEntity)
        {
            foreach (var error in eventUtils.ExtendedValidateEventEntity(newEntity, oldEntity))
                ModelState.AddModelError(error.Key, error.Value);
        }

        private void SetEventImage(HttpPostedFileBase file, Event entity)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = string.Format("{0}-{1}{2}", entity.HtmlName, Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName));
                string fullPath = Server.MapPath("~/Images/Events/") + fileName;

                Image image = null;
                Image listImage = null;
                try
                {
                    image = Image.FromStream(file.InputStream);
                    listImage = ResizeImage.ResizeImageIfNeeded(image, 200, 200);
                    listImage.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    entity.Photo = fileName;
                    repository.Save();
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Akce.SetEventImage");
                }
                finally
                {
                    if (image != null)
                        image.Dispose();
                    if (listImage != null)
                        listImage.Dispose();
                }
            }
        }

        private EventModel PrepareModel(EventModel model)
        {
            SetOrganisators(model);
            SetCategories(model);

            return model;
        }

        private void SetOrganisators(EventModel model)
        {
            if (model.Organisators == null || !model.Organisators.Any())
            {
                if (string.IsNullOrWhiteSpace(model.OrganisatorNames))
                {
                    // no organisators => current user is organisator
                    model.OrganisatorNames = User.Identity.Name;
                    model.Organisators = new[] { new User(repository.One<AspNetUsers>(u => u.UserName == model.OrganisatorNames)) };
                }
                else
                {
                    // create organisator from their names
                    var names = model.OrganisatorNames.Split(',').Select(o => Convert.ToInt32(o));
                    model.Organisators = names.Select(n => new User(repository.One<AspNetUsers>(u => u.Id == n))).ToArray();
                }
            }
            else
            {
                // set organisator names
                model.OrganisatorNames = string.Join(",", model.Organisators.Select(o => o.name));
            }
        }

        private void SetOrganisators(Event entity, EventModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OrganisatorNames))
            {
                ModelState.AddModelError("", "U akce musí být alespoň jeden organizátor");
                return;
            }

            var ids = model.OrganisatorNames.Split(',').Select(o => Convert.ToInt32(o)).ToArray();
            foreach(var id in ids)
            {
                if (entity.EventOrganisator.All(o => o.Id_User != id))
                {
                    var user = repository.One<AspNetUsers>(id);
                    entity.EventOrganisator.Add(new EventOrganisator() { Id_User = user.Id, Event = entity });
                }
            }

            foreach (var idOrganisator in entity.EventOrganisator.Select(e => e.Id_User).ToArray())
            {
                if (!ids.Contains(idOrganisator))
                {
                    var organisator = entity.EventOrganisator.Single(o => o.Id_User == idOrganisator);
                    repository.Delete(organisator);
                }
            }

            if (entity.EventOrganisator == null || entity.EventOrganisator.Count == 0)
                ModelState.AddModelError("", "U akce musí být alespoň jeden organizátor");
        }

        private void SetInvitation(Event entity)
        {
            if (entity.IsPrivate)
            {
                foreach(var org in entity.EventOrganisator)
                {
                    entity.EventInvitation.Add(new EventInvitation()
                    {
                        Event = entity,
                        Id_User = org.Id_User,
                    });
                }

                repository.Save();
            }
        }

        private void PublicateEvent(Event entity)
        {
            if (entity.State != EventStateEnum.Active)
                return;

            // check if event must be confirmed
            // - if is it user's first event - not implemented yet
            // - if event is oficial and too expensive 
            if (entity.EventType == EventTypeEnum.PlacenaSdruzenim && entity.Costs > 10000 && !User.IsInRole(RolesEnum.Council.ToString()))
            {
                CreateToastrMessage("Akce byla vytvořena a čeká na schválení vedením.");
                entity.State = EventStateEnum.ForAcceptance;
                repository.Save();

                SendMessageToCouncil(entity);
                return;
            }

            if (!User.IsInRole(Strings.EventCreator))
            {
                CreateToastrMessage("Akce byla vytvořena a čeká na schválení hlavními organizátory.");
                entity.State = EventStateEnum.ForAcceptance;
                repository.Save();

                SendMessageToEventConfirmators(entity);
                return;
            }

            if (entity.State == EventStateEnum.Active && entity.EventType != EventTypeEnum.Soukroma)
            {
                try
                {
                    // publicate only nonprivate and active events
                    eventUtils.Publicate(entity, GetUser);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Akce.PublicateEvent");
                }

                CreateToastrMessage("Akce byla v pořádku zveřejněna.");
            }

            // log user to active event (active = not InWork and not ForAcceptance) and not only suggested event
            if (entity.State == EventStateEnum.Active && entity.EventType != EventTypeEnum.TipNaAkci)
                eventUtils.WorkFlow(entity, GetUser);
        }

        private void SendMessageToCouncil(Event entity)
        {
            SendMessageToConfirm(entity, (int)RolesEnum.Council);
        }

        private void SendMessageToEventConfirmators(Event entity)
        {
            SendMessageToConfirm(entity, (int) RolesEnum.EventConfirmator);
        }

        private void SendMessageToConfirm(Event entity, int roleId)
        {
            var councils = repository.One<AspNetRoles>(roleId).AspNetUsers;
            var message = new Message()
            {
                AspNetUsers = GetSystemUser,
                Id_Sender = GetSystemUser.Id,
                Subject = "Nová akce ke schválení",
                Text = string.Format("Nová akce ke schválení <a href=\"{0}\">{1}</a>",
                    new Uri(Request.Url, Url.Content("~/Akce/Detail/" + entity.HtmlName)),
                    entity.Name),
                Time = Info.CentralEuropeNow,
                Visible = true,
                Id_ReplyMessage = null,
                MessageRecipient = councils.Select(r => new MessageRecipient()
                {
                    Id_User = r.Id,
                    Unreaded = true,
                    Visible = true,
                }).ToArray(),
            };

            messageService.SendMessage(message);
        }

        private void CopyEventProperties(Event dbEntity, Event entity)
        {
            if (entity.Costs != dbEntity.Costs)
                logger.LogInfo(string.Format("Náklady u akce {0} změněny z {1} na {2}", dbEntity.HtmlName, dbEntity.Costs, entity.Costs), "Akce.CopyEventProperties");

            dbEntity.Capacity = entity.Capacity;
            dbEntity.Contact = entity.Contact;
            dbEntity.Description = entity.Description;
            dbEntity.LastPaidDateTime = entity.LastPaidDateTime;
            dbEntity.LastSignINDateTime = entity.LastSignINDateTime;
            dbEntity.Link = entity.Link;
            dbEntity.MayBeLogOn = entity.MayBeLogOn;
            dbEntity.MeetDateTime = entity.MeetDateTime;
            dbEntity.MeetPlace = entity.MeetPlace;
            dbEntity.Name = entity.Name;
            dbEntity.Perex = entity.Perex;
            dbEntity.Place = entity.Place;
            dbEntity.Price = entity.Price;
            dbEntity.StartDateTime = entity.StartDateTime;
            dbEntity.StopDateTime = entity.StopDateTime;
            dbEntity.MinimumParticipants = entity.MinimumParticipants;
            dbEntity.Costs = entity.Costs;
            dbEntity.CostsDescription = entity.CostsDescription;
            dbEntity.MainCategory = entity.MainCategory;
            dbEntity.IsStay = entity.IsStay;
            repository.Save();
        }

        private void SetCategories(EventModel model)
        {
            var allCategories = repository.All<EventCategory>();
            foreach (var category in allCategories)
            {
                model.Categories.Add(new Category()
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsAssigned = model.CategoryIds.Contains(category.Id),
                    MainCategory = category.MainCategory
                });
            }
        }

        private void UpdateCategories(int[] selectedCategories, Event eventEntity)
        {
            if (selectedCategories == null)
            {
                return;
            }

            var selectedCategoriesIds = new HashSet<int>(selectedCategories);
            var eventCategories = new HashSet<int>(eventEntity.EventCategories.Select(c => c.Id));

            foreach (var category in repository.All<EventCategory>())
            {
                if (selectedCategoriesIds.Contains(category.Id))
                {
                    if (!eventCategories.Contains(category.Id))
                    {
                        eventEntity.EventCategories.Add(category);
                    }
                }
                else
                {
                    if (eventCategories.Contains(category.Id))
                    {
                        eventEntity.EventCategories.Remove(category);
                    }
                }
            }
        }

        #endregion

        #region Duplicate Event
        [Authorize]
        public ActionResult Duplikace(string id)
        {
            var entity = repository.One<Event>(ev => ev.HtmlName == id);
            var model = new DuplicateEvent()
            {
                Name = entity.Name,
                Days = 7,
                NewStartDate = entity.StartDateTime,
                Title = "Duplikace akce " + entity.Name,
                HtmlName = entity.HtmlName,
            };

            model.AddActionButton("Zpět na akci", Utilities.Url("~/Akce/Detail/" + id), "icon-arrow-left");

            return View(model);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken, ActionName("Duplikace")]
        public ActionResult DuplikacePost(string id, DuplicateEvent model, string submitButton)
        {
            if (submitButton == "Ještě editovat akci")
            {
                var eventEntity = repository.OneByHtml<Event>(model.HtmlName).ToViewModel();
                eventEntity.OldHtml = id;
                eventEntity.Name = model.Name;

                if (model.DuplicationType == "days")
                {
                    MoveEventModel(eventEntity, model.Days);
                }

                if (model.DuplicationType == "date")
                {
                    int moveDays = (model.NewStartDate.Date - eventEntity.Start.Date).Days;
                    MoveEventModel(eventEntity, moveDays);
                }

                PrepareModel(eventEntity);

                TempData[$"duplicateEvent_{GetUserId}"] = eventEntity;
                return RedirectToAction("DuplikaceEdit", "Akce");
            }

            if (submitButton == "Vyhlásit akci")
            {
                int days = 0;
                var oldEntity = repository.OneByHtml<Event>(id);

                if (model.DuplicationType == "date")
                {
                    days = (model.NewStartDate.Date - oldEntity.StartDateTime.Date).Days;
                }
                else if (model.DuplicationType == "days")
                {
                    days = model.Days;
                }
                else
                {
                    ModelState.AddModelError("", "Nepodařilo se určit o kolik dnů se má akce posunout.");
                }

                if (oldEntity.StartDateTime.AddDays(days) <= Info.CentralEuropeNow)
                    ModelState.AddModelError("", "Datum začátku akce je v minulosti nebo v neplatném formátu.");

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var newEntity = eventUtils.DuplicateEvent(oldEntity, days, model.Name);

                if (newEntity.State == EventStateEnum.Deleted)
                    newEntity.State = EventStateEnum.Active;

                repository.SetHtmlName(newEntity, model.Name);
                repository.Add(newEntity);
                SetInvitation(newEntity);
                eventUtils.WorkFlow(newEntity, GetUser);
                eventUtils.Publicate(newEntity, GetUser);

                CreateToastrMessage("Akce byla v pořádku zduplikována.");

                return RedirectToAction("Detail", "Akce", new { id = newEntity.HtmlName });
            }

            return View(model);
        }

        [Authorize(Roles = Strings.EventCreator)]
        public ActionResult DuplikaceEdit()
        {
            EventModel model = TempData[$"duplicateEvent_{GetUserId}"] as EventModel;
            return View(model);
        }

        [Authorize(Roles = Strings.EventCreator)]
        [HttpPost, ValidateAntiForgeryToken, ActionName("DuplikaceEdit")]
        public ActionResult DuplikaceEditPost(HttpPostedFileBase file, EventModel model)
        {
            if (!ModelState.IsValid)
                return View(PrepareModel(model));

            try
            {
                if (model.EventType == EventTypeEnum.TipNaAkci)
                {
                    model.MeetPlace = null;
                    model.MeetTime = null;
                    model.OrganisatorNames = GetUserId.ToString();
                }

                // create event
                var entity = model.ToEntity();

                ExtendedValidateEventEntity(entity, null);
                SetOrganisators(entity, model);
                if (!ModelState.IsValid)
                    return View(PrepareModel(model));

                repository.SetHtmlName(entity, entity.Name);

                var discussionEntity = new Discussion() { Name = model.Name, HtmlName = entity.HtmlName, IsAlone = false };
                repository.Add(discussionEntity);
                entity.Id_Discussion = discussionEntity.Id;

                UpdateCategories(model.CategoryIds, entity); // set category

                if (file != null && file.ContentLength > 0)
                {
                    SetEventImage(file, entity);
                }
                else
                {
                    var oldEntity = repository.OneByHtml<Event>(model.OldHtml);
                    entity.Photo = oldEntity.Photo;
                }

                entity.AccountSymbol = repository.All<Event>().Max(ev => ev.AccountSymbol) + 1;
                repository.Add(entity);
                SetInvitation(entity);

                eventUtils.WorkFlow(entity, GetUser);
                if (entity.EventType == EventTypeEnum.PlacenaSdruzenim && entity.Costs > 10000 && !User.IsInRole(RolesEnum.Council.ToString()))
                {
                    CreateToastrMessage("Akce byla vytvořena a čeká na schválení vedením.");
                    entity.State = EventStateEnum.ForAcceptance;
                    repository.Save();

                    SendMessageToCouncil(entity);
                }
                else
                {
                    eventUtils.Publicate(entity, GetUser);
                    CreateToastrMessage("Akce byla v pořádku vytvořena.");
                }

                return RedirectToAction("Detail", "Akce", new { id = entity.HtmlName });
            }
            catch (Exception ex)
            {
                LogValidationErrors(ex, "Akce.DuplikaceEditPost");
                logger.LogException(ex, "Akce.DuplikaceEditPost");
            }
            return View(PrepareModel(model));
        }

        private void MoveEventModel(EventModel model, int days)
        {
            model.Start = model.Start.AddDays(days);
            model.Stop = model.Stop.AddDays(days);
            model.MeetTime = model.MeetTime.Value.AddDays(days);
            if (model.LastSignTime.HasValue)
                model.LastSignTime = model.LastSignTime.Value.AddDays(days);
            if (model.LastPaidTime.HasValue)
                model.LastPaidTime = model.LastPaidTime.Value.AddDays(days);
        }
        #endregion

        #region Event detail and login to event
        public ActionResult Detail(string id)
        {
            var eventEntity = eventRepository.GetEventDetail(id);

            #region check rights to event
            if (eventEntity.EventType == (int)EventTypeEnum.Soukroma)
            {
                if (!User.Identity.IsAuthenticated)
                    return RedirectToHome();

                if (eventRepository.GetInvitedUsers(eventEntity.Id).All(u => u.Id != GetUserId))
                    return RedirectToHome();
            }

            if (eventEntity.State == (int)EventStateEnum.InWork)
            {
                if (eventEntity.Organisators.All(o => o.Id != GetUserId))
                {
                    return RedirectToHome();
                }
            }

            if (eventEntity.State == (int)EventStateEnum.ForAcceptance)
            {
                // k akce ke schvaleni maji pristup jen organizatori a schvalovatele
                if (!(eventEntity.Organisators.Any(eo => eo.Id == GetUserId)
                      || (eventEntity.EventType == (int)EventTypeEnum.Bezna && User.IsInRole(Strings.EventConfirmator))
                      || (eventEntity.EventType == (int)EventTypeEnum.TipNaAkci && User.IsInRole(Strings.EventConfirmator))
                      || (eventEntity.EventType == (int)EventTypeEnum.OficialniSdruzeni && User.IsInRole(Strings.Council))))
                {
                    return RedirectToHome();
                }
            }            
            #endregion

            var viewModel =
                new GenericBaseViewModel<EventDetailViewModel>(EventDetailViewModel.FromModel(eventEntity));

            if (User.Identity.IsAuthenticated)
            {
                viewModel.Entity.IsOrganisator = viewModel.Entity.Organisators.Any(o => o.Id == GetUserId);
                viewModel.Entity.UserEventLogin = viewModel.Entity.Participants
                                                      .FirstOrDefault(p => p.User.Id == GetUserId && !p.IsExternal)
                                                      ?.LoginStatus
                                                  ?? (int) EventLoginEnum.Nevyjadreno;

                viewModel.Entity.ShowAccountSymbol = viewModel.Entity.ShowAccountSymbol || User.IsInRole(RolesEnum.Accountant.ToString())
                    || User.IsInRole(RolesEnum.Administrator.ToString()) || User.IsInRole(RolesEnum.Council.ToString())
                    || User.IsInRole(RolesEnum.Supervisor.ToString());

                logger.LogInfo(string.Format("Uživatel(ka) {0} navštívil(a) akci {1}", User.Identity.Name, viewModel.Entity.Name), "Akce.Detail");

                viewModel.Entity.GoogleCalendarUrl = string.Format("http://www.google.com/calendar/event?action=TEMPLATE&text={0}&dates={1}/{2}&sprop=website:{3}&details={4}&location={5}",
                        Uri.EscapeUriString(viewModel.Entity.Name),
                        viewModel.Entity.Start.ToString("yyyyMMddTHHmmss"),
                        viewModel.Entity.Stop.ToString("yyyyMMddTHHmmss"),
                        Utilities.Url("~/Akce/Detail/" + viewModel.Entity.HtmlName, true),
                        Uri.EscapeUriString(WebUtility.HtmlDecode(viewModel.Entity.Perex) + Environment.NewLine) + Utilities.Url("~/Akce/Detail/" + viewModel.Entity.HtmlName, true),
                        Uri.EscapeUriString(WebUtility.HtmlDecode(viewModel.Entity.Place)));
            }

            AddActionButtons(viewModel);

            ViewBag.Description = viewModel.Entity.Perex;
            if (viewModel.Entity.Photo != null)
                ViewBag.Image = new Uri(Request.Url, Url.Content("~/Images/Events/" + viewModel.Entity.Photo));

            return View(viewModel);
        }

        public ActionResult Detaily(string id)
        {
            return RedirectToAction("Detail", new { id });
        }

        public ActionResult DetailById(int id)
        {
            var view = Detail(repository.One<Event>(id).HtmlName) as ViewResult;
            return View("Detail", view.Model);
        }

        private void AddActionButtons(GenericBaseViewModel<EventDetailViewModel> model)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (model.Entity.IsOrganisator || User.IsInRole(Strings.Administrator))
                {
                    model.AddActionButton("Účastníci", Utilities.Url("~/Akce/Ucastnici/", model.Entity.HtmlName, HttpContext), "icon-check");
                    model.AddActionButton("Duplikovat akci", Utilities.Url("~/Akce/Duplikace/", model.Entity.HtmlName, HttpContext), "icon-copy");

                    if (!model.Entity.IsInPastOrDeleted)
                    {
                        model.AddActionButton("Editovat", Utilities.Url("~/Akce/Edit/", model.Entity.HtmlName, HttpContext), "icon-edit");
                    }
                    model.AddActionButton("Vzkaz organizátora", Utilities.Url("~/Akce/Vzkaz/", model.Entity.HtmlName, HttpContext), "icon-envelope-alt");

                    if (!model.Entity.IsDeleted)
                        model.AddActionButton("Smazat akci", Utilities.Url("~/Akce/Smazat/", model.Entity.HtmlName, HttpContext), "icon-trash");

                    if (model.Entity.EventType == (int)EventTypeEnum.TipNaAkci)
                        model.AddActionButton("Převést na akci", Utilities.Url("~/Akce/NaAkci/", model.Entity.HtmlName, HttpContext), "icon-copy");
                }

                if (model.Entity.EventType == (int)EventTypeEnum.PlacenaSdruzenim &&
                    (model.Entity.IsOrganisator
                     || User.IsInRole(Strings.Council)
                     || User.IsInRole(Strings.Supervisor)
                     || User.IsInRole(Strings.Administrator)))
                {
                    model.AddActionButton("Finance akce", Utilities.Url("~/Akce/Finance/", model.Entity.HtmlName, HttpContext), "icon-money");
                }

                if (model.Entity.IsPrivate)
                {
                    model.AddActionButton("Pozvaní účastníci", Utilities.Url("~/Akce/Pozvat/", model.Entity.HtmlName, HttpContext), "icon-share");
                }

                if (model.Entity.State == (int)EventStateEnum.ForAcceptance)
                {
                    if (model.Entity.EventType == (int)EventTypeEnum.Bezna && User.IsInRole(Strings.EventConfirmator)
                        || model.Entity.EventType == (int)EventTypeEnum.TipNaAkci && User.IsInRole(Strings.EventConfirmator)
                        || (model.Entity.EventType == (int)EventTypeEnum.OficialniSdruzeni && User.IsInRole(Strings.Council)))
                    {
                        model.AddActionButton("Schvalování",
                            Utilities.Url("~/Akce/Schvalit/", model.Entity.HtmlName, HttpContext), "icon-ok");
                    }
                }

                if (model.Entity.IsInPastOrDeleted)
                {
                    model.AddActionButton("Přidat fotogalerii", Utilities.Url("~/Akce/Fotky/", model.Entity.HtmlName, HttpContext), "icon-picture");

                    model.Entity.CanAddSummary = model.Entity.Summary != null
                                          && (User.IsInRole(Strings.Administrator)
                                              || model.Entity.Participants.Any(p => p.User.Id == GetUserId && p.LoginStatus == (int)EventLoginEnum.Prijdu));
                }
                else
                {
                    model.AddActionButton("Správa externistů", Utilities.Url("~/Akce/Externisti/", model.Entity.HtmlName, HttpContext), "icon-meh");
                }
            }
        }


        [Authorize, HttpPost]
        public ActionResult Login(EventUserLog eventUserLog)
        {
            logger.LogInfo($"Uživatel {User.Identity.Name} chce změnit stav u akce {eventUserLog.IdEvent} na stav {eventUserLog.StatusEnum}", "Akce.Login");
            var eventEntity = repository.One<Event>(eventUserLog.IdEvent);
            logToEvent.LogToEvent(eventEntity, GetUser, eventUserLog.StatusEnum, true);

            var model = eventRepository.GetEventParticipantsPart(eventUserLog.IdEvent);
            var viewModel = EventDetailViewModel.FromModel(model);
            viewModel.UserEventLogin = viewModel.Participants
                                                  .FirstOrDefault(p => p.User.Id == GetUserId && !p.IsExternal)
                                                  ?.LoginStatus
                                              ?? (int)EventLoginEnum.Nevyjadreno;

            return PartialView("_ParticipantsPart", viewModel);
        }
        #endregion

        #region Participants
        [Authorize]
        public ActionResult Ucastnici(string id)
        {
            var eventEntity = repository.One<Event>(e => e.HtmlName == id);

            var model = new EventParticipantHistoryOverview()
            {
                ParticipantHistory = repository.Where<EventParticipantHistory>(eph => eph.Id_Event == eventEntity.Id)
                    .ToArray()
                    .Select(eph => new ParticipantHistory(eph).ToArray()),
                Name = eventEntity.Name,
                HtmlName = eventEntity.HtmlName,
                Id_Event = eventEntity.Id,
                IsFinished = (eventEntity.StopDateTime <= Info.CentralEuropeNow),
                LoginParticipants = repository.Where<EventParticipant>(ep => ep.Id_Event == eventEntity.Id)
                    .OrderBy(ep => ep.Time)
                    .Select(ep => new Participant()
                    {
                        LoginStatus = ep.EventLoginStatus,
                        User = new User()
                        {
                            name = (ep.IsExternal) ? "Externí účastník od " + ep.AspNetUsers.UserName : ep.AspNetUsers.UserName,
                            id = ep.AspNetUsers.Id.ToString(),
                        },
                        Time = ep.Time,
                        IsExternal = ep.IsExternal,
                        Id = ep.Id,
                    })
                    .ToArray(),
                Title = "Správa účastníků akce " + eventEntity.Name,
                IsAdministrator = User.IsInRole(Strings.Administrator),
            };

            model.AddActionButton("Zpět na akci", Utilities.Url("~/Akce/Detail/" + id), "icon-arrow-left");

            return View(model);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken, ActionName("AddUsers")]
        public ActionResult AddUsers(string id, string users)
        {
            var userIds = users.Split(',');

            foreach(var userId in userIds)
            {
                var userName = repository.One<AspNetUsers>(userId).UserName;
                logger.LogInfo(string.Format("Uživatel {0} přidal uživatele {1} k akci {2}",
                    User.Identity.Name,
                    userName,
                    id), "Akce.AddUsers");
                var eventEntity = repository.OneByHtml<Event>(id);
                var user = repository.One<AspNetUsers>(u => u.UserName == userName);
                logToEvent.LogToEvent(eventEntity, user, EventLoginEnum.Prijdu, false);
            }
            CreateToastrMessage("Uživatelé byli přidáni k akci");

            return RedirectToAction("Detail", "Akce", new { id });
        }

        [Authorize]
        public ActionResult RefuseUser(int id)
        {
            var participant = repository.One<EventParticipant>(id);

            if (UserCanChangeAction(participant.Event))
            {
                logger.LogInfo(string.Format("Uživatel {0} vyřadil uživatele {1} z akce {2}",
                    User.Identity.Name,
                    participant.AspNetUsers.UserName,
                    participant.Event.HtmlName),
                    "Akce.RefuseUser");
                logToEvent.RefuseUser(participant);
            }

            return RedirectToAction("Ucastnici", new { id = participant.Event.HtmlName });
        }

        [Authorize]
        public ActionResult ExcuseUser(int id)
        {
            var participant = repository.One<EventParticipant>(id);

            if (UserCanChangeAction(participant.Event))
            {
                if (participant != null)
                {
                    participant.EventLoginStatus = EventLoginEnum.Omluven;
                    repository.Save();
                    logger.LogInfo(string.Format("Uživatel {0} omluvil uživatele {1} na akce {2}",
                        User.Identity.Name,
                        participant.AspNetUsers.UserName,
                        participant.Event.HtmlName),
                        "Akce.ExcuseUser");
                }
            }

            return RedirectToAction("Ucastnici", new { id = participant.Event.HtmlName });
        }

        [Authorize]
        public ActionResult UnExcuseUser(int id)
        {
            var participant = repository.One<EventParticipant>(id);

            if (UserCanChangeAction(participant.Event))
            {
                if (participant != null)
                {
                    participant.EventLoginStatus = EventLoginEnum.Neomluven;
                    repository.Save();
                    logger.LogInfo(string.Format("Uživatel {0} neomluvil uživatele {1} na akce {2}",
                        User.Identity.Name,
                        participant.AspNetUsers.UserName,
                        participant.Event.HtmlName),
                        "Akce.UnExcuseUser");
                }
            }

            return RedirectToAction("Ucastnici", new { id = participant.Event.HtmlName });
        }

        /// <summary>
        /// Uzivatel ma pravo editovat akci a jeji ucastniky (je v roli organizatora nebo administratora)
        /// </summary>
        private bool UserCanChangeAction(Event entity)
        {
            return (entity.EventOrganisator.Any(eo => eo.AspNetUsers.UserName == User.Identity.Name) || User.IsInRole(Strings.Administrator));
        }
        #endregion

        #region Finance
        [Organisator(Strings.Council, Strings.Administrator, Strings.Supervisor)]
        public async Task<ActionResult> Finance(string id)
        {
            var entity = repository.One<Event>(e => e.HtmlName == id);

            var model = new EventFinanceViewModel()
            {
                Name = entity.Name,
                HtmlName = entity.HtmlName,
                FirstCost = entity.Costs,
                EventCostsDescription = entity.CostsDescription,
                Payments = entity.Payment
                    .OrderBy(p => p.CreateDate)
                    .Select(p => new PaymentViewModel()
                    {
                        Amount = (decimal)p.Amount * (-1),
                        CreateDate = p.CreateDate,
                        PaidDate = p.UpdateDate,
                        UserName = p.AspNetUsers.UserName,
                        UserHtmlName = p.AspNetUsers.HtmlName,
                        Description = p.Note,
                        Status = p.Status,
                    })
                    .ToList(),
            };

            model.Title = $"Finance akce {model.Name}";
            model.Icon = "icon-money";
            model.Costs = await accounting.GetCosts(entity.AccountSymbol);
            model.AddActionButton("Zpět na akci", Utilities.Url("~/Akce/Detail/" + id), "icon-arrow-left");

            return View(model);
        }
        #endregion

        #region FinanceRefundace
        public ActionResult FinanceRefundace(string eventHtml, int sum, string userHtml)
        {
            var eventEntity = repository.OneByHtml<Event>(eventHtml);
            if (UserHasAccesToEventMoney(eventEntity))
            {
                var user = repository.OneByHtml<AspNetUsers>(userHtml);
                var participants = eventEntity.EventParticipant.Where(p => p.Id_User == user.Id);

                if (participants.Any())
                {
                    var paidAmount = participants.Select(p => p.Payment)
                        .Where(p => p.Status == PaymentStatus.Paid)
                        .Sum(p => p.Amount) * (-1);
                    if (paidAmount > 0)
                    {
                        var model = new RefundaceViewModel()
                        {
                            Amount = Convert.ToInt32(paidAmount),
                            UserName = user.UserName,
                            UserHtml = user.HtmlName,
                            EventHtml = eventEntity.HtmlName,
                        };
                        return PartialView(model);
                    }
                    else
                    {
                        CreateToastrMessage("Nenalezeny platby k danému uživateli.");
                    }
                }
                else
                {
                    CreateToastrMessage($"Uživatel {user.UserName} se nezúčasnil akce.");
                }
            }
            else
            {
                CreateToastrMessage("Nemáte oprávnění k této akci.");
            }

            return RedirectToAction("Finance", "Akce", new { id = eventEntity.HtmlName });
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName("FinanceRefundace")]
        public ActionResult FinanceRefundacePost(RefundaceViewModel model)
        {
            var eventEntity = repository.OneByHtml<Event>(model.EventHtml);
            if (model.Amount <= 0)
            {
                CreateToastrMessage("Je třeba zadat kladnou částku.");
            }
            else if (UserHasAccesToEventMoney(eventEntity))
            {
                var user = repository.OneByHtml<AspNetUsers>(model.UserHtml);
                var participants = eventEntity.EventParticipant.Where(p => p.Id_User == user.Id &&
                                                                           (p.EventLoginStatus ==
                                                                            EventLoginEnum.Prijdu ||
                                                                            p.EventLoginStatus ==
                                                                            EventLoginEnum.PoTerminu ||
                                                                            p.EventLoginStatus ==
                                                                            EventLoginEnum.Neprijdu));

                if (participants.Any())
                {
                    var paidAmount = participants.Select(p => p.Payment)
                        .Where(p => p.Status == PaymentStatus.Paid)
                        .Sum(p => p.Amount) * (-1);
                    if (model.Amount <= paidAmount)
                    {
                        var newPayment = new Payment()
                        {
                            Amount = model.Amount,
                            CreateDate = Info.CentralEuropeNow,
                            Id_Event = eventEntity.Id,
                            Id_User = user.Id,
                            Note = $"Refundace za akci {eventEntity.Name}, refundováno {model.Amount}",
                        };

                        logger.LogInfo($"Uživatel {GetUser.UserName} vrátil uživateli {user.UserName} částku {model.Amount} u akce {eventEntity.Name}.", "Akce.FinanceRefundacePost");

                        repository.Add(newPayment);
                        repository.Save();
                        CreateToastrMessage("Platba byla úspěšně vložena");
                    }
                    else
                    {
                        CreateToastrMessage("Nelze vrátit více než bylo zaplaceno.");
                    }
                }
                else
                {
                    CreateToastrMessage($"Uživatel {user.UserName} se nezúčasnil akce.");
                }
            }
            else
            {
                CreateToastrMessage("Nemáte oprávnění k této akci.");
            }

            return RedirectToAction("Finance", "Akce", new { id = eventEntity.HtmlName });
        }

        private bool UserHasAccesToEventMoney(Event entity)
        {
            int userId = GetUserId;
            if (entity.EventType == EventTypeEnum.PlacenaSdruzenim &&
                    (entity.EventOrganisator.Any(o => o.Id_User == userId)
                     || User.IsInRole(Strings.Council)
                     || User.IsInRole(Strings.Supervisor)
                     || User.IsInRole(Strings.Administrator)))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Confirm Event
        [Authorize(Roles = Strings.Council+","+ Strings.CertifiedOrganisator)]
        public ActionResult Schvalit(string id)
        {
            var entity = repository.One<Event>(ev => ev.HtmlName == id);

            if (entity.EventType == EventTypeEnum.OficialniSdruzeni && !User.IsInRole(Strings.Council))
            {
                CreateToastrMessage("Nemáte právo schvalovat tento typ akce");
                return RedirectToAction("Index", "Home");
            }

            if (entity.State != EventStateEnum.ForAcceptance)
            {
                CreateToastrMessage("Akce již byla schválena či zamítnuta");
                return RedirectToAction("Detail", "Akce", new { id });
            }

            return View(entity.ToSimpleViewModel());
        }

        [Authorize(Roles = Strings.Council+","+ Strings.CertifiedOrganisator)]
        [HttpPost, ActionName("Schvalit")]
        public ActionResult SchvalitPost(string id, string button)
        { 
            var entity = repository.One<Event>(e => e.HtmlName == id);
            if (entity == null)
            {
                CreateToastrMessage("Akce nenalezena");
                logger.LogDebug(string.Format("Nenalezena akce s id:{0} ke schválení", id), "Akce.Schvalit");
                return RedirectToAction(string.Empty, string.Empty);
            }

            if (entity.EventType == EventTypeEnum.OficialniSdruzeni && !User.IsInRole(Strings.Council))
            {
                CreateToastrMessage("Nemáte právo schvalovat tento typ akce");
                return RedirectToAction("Index", "Home");
            }

            if (entity.State == EventStateEnum.ForAcceptance)
            {
                bool ok = (button == "schvalit" || button == "schvalitKomercni");

                entity.State = (ok) ? EventStateEnum.Active : EventStateEnum.Rejected;
                repository.Save();

                if (ok)
                {
                    logger.LogInfo(string.Format("Uživatel {0} schválil akci {1}", User.Identity.Name, id), "Akce.Schvalit");
                    CreateToastrMessage("Akce byla úspěšně schválena");
                    SendMessageToOrganisators(entity, "byla schálena");

                    if (button == "schvalitKomercni")
                    {
                        entity.EventType = EventTypeEnum.Komercni;
                        repository.Save();
                    }

                    // log user and publicate event
                    eventUtils.WorkFlow(entity, GetUser);
                    eventUtils.Publicate(entity, GetUser, true);
                }
                else
                {
                    logger.LogInfo(string.Format("Uživatel {0} zamítl akci {1}", User.Identity.Name, id), "Akce.Schvalit");
                    CreateToastrMessage("Akce byla úspěšně zamítnuta");
                    SendMessageToOrganisators(entity, "byla zamítnuta");
                }
            }
            else
            {
                CreateToastrMessage("Akce již byla schválena či zamítnuta");
                logger.LogDebug(string.Format("Akce {0} již byla rozhodnuta", id), "Akce.Schvalit");
            }

            return RedirectToAction("Detail", "Akce", new { id = entity.HtmlName });
        }

        private void SendMessageToOrganisators(Event entity, string result)
        {
            var organisators = entity.EventOrganisator.Select(o => o.AspNetUsers);
            var message = new Message()
            {
                AspNetUsers = GetSystemUser,
                Id_Sender = GetSystemUser.Id,
                Subject = string.Format("Akce {0} {1}", entity.Name, result),
                Text = string.Format("Vaše akce <a href=\"{0}\">{1}</a> {2}",
                    new Uri(Request.Url, Url.Content("~/Akce/Detail/" + entity.HtmlName)),
                    entity.Name,
                    result),
                Time = Info.CentralEuropeNow,
                Visible = true,
                Id_ReplyMessage = null,
                MessageRecipient = organisators.Select(r => new MessageRecipient()
                {
                    Id_User = r.Id,
                    Unreaded = true,
                    Visible = true,
                }).ToArray(),
            };

            try
            {
                messageService.SendMessage(message);
            }
            catch(Exception ex)
            {
                logger.LogException(ex, "Akce.SendMessageToOrganisators");
            }
        }

        #endregion

        #region Delete event
        [Organisator(Strings.Administrator)]
        public ActionResult Smazat(string id)
        {
            var entity = repository.One<Event>(e => e.HtmlName == id);
            SimpleEventModel model = entity.ToSimpleViewModel();
            model.Title = "Smazat akci " + model.Name;

            return View(model);
        }

        [Organisator(Strings.Administrator)]
        [HttpPost, ActionName("Smazat"), ValidateAntiForgeryToken]
        public ActionResult SmazatPost(string id)
        {
            var entity = repository.One<Event>(e => e.HtmlName == id);
            entity.State = EventStateEnum.Deleted;
            repository.Save();

            if (entity.EventType == EventTypeEnum.PlacenaSdruzenim)
            {
                // cancel all unpaied payments
                var paidPayments = entity.Payment.Where(p => p.Status == PaymentStatus.Active);
                foreach (var payment in paidPayments)
                {
                    payment.Status = PaymentStatus.Cancelled;
                }

                // refund all participants
                var participants = entity.EventParticipant.Where(p => p.EventLoginStatus == EventLoginEnum.Prijdu);
                foreach (var participant in participants)
                {
                    repository.Add(new Payment()
                    {
                        Amount = (participant.Payment == null) ? entity.Price : participant.Payment.Amount * (-1),
                        AspNetUsers = participant.AspNetUsers,
                        CreateDate = Info.CentralEuropeNow,
                        Event = entity,
                        guid = Guid.NewGuid(),
                        Id_Event = entity.Id,
                        Id_User = participant.AspNetUsers.Id,
                        Note = "Vrácení za zrušenou akci " + entity.Name,
                        Status = PaymentStatus.Active,

                    });
                }

                repository.Save();
            }

            if (!entity.IsPrivate)
                activityListService.AddActivity(ActivityCreator.Create(GetUser, entity, ActivityType.DeleteEvent));
            CreateToastrMessage("Akce byla úspěšně smazána");

            return RedirectToAction("Detail", "Akce", new { id = entity.HtmlName });
        }
        #endregion

        #region Move to regular action

        [Organisator(Strings.Administrator)]
        public ActionResult NaAkci(string id)
        {
            var entity = repository.One<Event>(e => e.HtmlName == id);
            SimpleEventModel model = entity.ToSimpleViewModel();
            model.Title = "Převést na akci " + model.Name;

            return View(model);
        }

        [Organisator(Strings.Administrator)]
        [HttpPost, ActionName("NaAkci"), ValidateAntiForgeryToken]
        public ActionResult NaAkciPost(string id)
        {
            var entity = repository.One<Event>(e => e.HtmlName == id);
            entity.EventType = EventTypeEnum.Bezna;
            repository.Save();

            PublicateEvent(entity);
            CreateToastrMessage("Akce byla úspěšně převedena.");

            return RedirectToAction("Detail", "Akce", new { id = entity.HtmlName });
        }
        #endregion

        #region Externists
        [Authorize]
        public ActionResult Externisti(string id)
        {
            return View(CreateExternistModel(id, 0));
        }

        [Authorize]
        [HttpPost, ActionName("Externisti"), ValidateAntiForgeryToken]
        public ActionResult ExternistiPost(string id, EventWithExternParticipant model)
        {
            if (!ModelState.IsValid)
                return View(CreateExternistModel(id, model.Count));

            var entity = repository.One<Event>(e => e.HtmlName == id);

            for (int i = 0; i < model.Count; i++)
            {
                logToEvent.LogExternalsToEvent(entity, GetUser);
            }

            if (model.Count > 0)
                CreateToastrMessage("Počet přidaných externích účastníků: " + model.Count.ToString());

            logger.LogInfo(string.Format("Uživatel(ka) {0} přidal {1} externistů k akci {2}", 
                User.Identity.Name,
                model.Count,
                entity.HtmlName), "Akce.ExternistiPost");

            return RedirectToAction("Detail", "Akce", new { id = entity.HtmlName });
        }

        private EventWithExternParticipant CreateExternistModel(string eventHtmlName, int externistCount)
        {
            var entity = repository.One<Event>(e => e.HtmlName == eventHtmlName);
            var model = new EventWithExternParticipant()
            {
                ExternParticipants = entity.EventParticipant
                    .Where(ep => ep.Id_User == GetUserId && ep.IsExternal)
                    .Select(ep => new ExternParticipant()
                    {
                        Id = ep.Id,
                        Time = ep.Time,
                        IsActive = (ep.EventLoginStatus == EventLoginEnum.Prijdu || ep.EventLoginStatus == EventLoginEnum.Nahradnik || ep.EventLoginStatus == EventLoginEnum.Rezervace || ep.EventLoginStatus== EventLoginEnum.NepotvrzenaRezervace),
                        NeedConfirmation = (ep.EventLoginStatus == EventLoginEnum.Nahradnik || ep.EventLoginStatus == EventLoginEnum.Rezervace || ep.EventLoginStatus == EventLoginEnum.NepotvrzenaRezervace),
                Status = ep.EventLoginStatus.GetDescription(),                        
                    }),
                Name = entity.Name,
                HtmlName = entity.HtmlName,
                Count = externistCount,
                Title = "Správa externistů u akce " + entity.Name,
            };

            model.Buttons.Add(new ActionButton("Zpět na akci", Utilities.Url("~/Akce/Detail/" + eventHtmlName), "icon-arrow-left"));

            return model;
        }

        [Authorize]
        public ActionResult OdebratExternistu(int id)
        {
            var participant = repository.One<EventParticipant>(id);
            logger.LogInfo(string.Format("Uživatel(ka) {0} odebral externistu s id {1}",
                User.Identity.Name,
                id), "Akce.OdebratExternistu");

            if (participant.AspNetUsers == GetUser)
            {
                logToEvent.LogExternalsFromEvent(participant);
                CreateToastrMessage("Externí účastník byl odebrán");

                return RedirectToAction("Externisti", "Akce", new { id = participant.Event.HtmlName });
            }
            else
            {
                CreateToastrMessage("Odhlašujete nesprávného účastníka");
                return RedirectToAction("Detail", "Akce", new { id = participant.Event.HtmlName });
            }
        }

        [Authorize]
        public ActionResult PrihlasitExternistu(int id)
        {
            var participant = repository.One<EventParticipant>(id);
            logger.LogInfo(string.Format("Uživatel(ka) {0} potvrdil externistu s id {1}",
                User.Identity.Name,
                id), "Akce.PrihlasitExternistu");

            if (participant.AspNetUsers == GetUser)
            {
                logToEvent.ConfirmExternalToEvent(participant);
                CreateToastrMessage("Externí účastník byl přihlášen");

                return RedirectToAction("Externisti", "Akce", new { id = participant.Event.HtmlName });
            }
            else
            {
                CreateToastrMessage("Přihlašujete nesprávného účastníka");
                return RedirectToAction("Detail", "Akce", new { id = participant.Event.HtmlName });
            }
        }
        #endregion

        #region OrganisatorMessage
        [Authorize]
        public ActionResult Vzkaz(string id)
        {
            var model = new OrganisatorMessage() { HtmlName = id, Title = "Vzkaz organizátora akce" };
            model.AddActionButton("Zpět na akci", Utilities.Url("~/Akce/Detail/" + id), "icon-arrow-left");

            return View(model);
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Vzkaz")]
        public ActionResult VzkazPost(string id, OrganisatorMessage model)
        {
            model.Title = "Vzkaz organizátora akce";
            if (!(model.Mozna || model.Nahradnik || model.NepotvrzenaRezervace || model.Prijdu || model.Rezervace || model.PoTerminu || model.RezervacePropadla))
                ModelState.AddModelError("", "Musí být vybrán alespoň jeden příjemce.");

            if (!ModelState.IsValid)
                return View(model);

            var eventEntity = repository.One<Event>(ev => ev.HtmlName == id);

            var statuses = new List<EventLoginEnum>();
            if (model.Mozna) statuses.Add(EventLoginEnum.Mozna);
            if (model.Nahradnik) statuses.Add(EventLoginEnum.Nahradnik);
            if (model.NepotvrzenaRezervace) statuses.Add(EventLoginEnum.NepotvrzenaRezervace);
            if (model.Prijdu) statuses.Add(EventLoginEnum.Prijdu);
            if (model.Rezervace) statuses.Add(EventLoginEnum.Rezervace);
            if (model.PoTerminu) statuses.Add(EventLoginEnum.PoTerminu);
            if (model.RezervacePropadla) statuses.Add(EventLoginEnum.RezervacePropadla);

            var recipients = eventEntity.EventParticipant
                .Where(ep => statuses.Contains(ep.EventLoginStatus))
                .Select(ep => ep.AspNetUsers)
                .Distinct();

            var entity = new Message()
            {
                AspNetUsers = GetUser,
                Id_Sender = GetUserId,
                Subject = string.Format("Zpráva organizátora akce: {0}", eventEntity.Name).TakeSafetely(50),
                Text = model.Message.ToHtml(),
                Time = Info.CentralEuropeNow,
                Visible = true,
                Id_ReplyMessage = null,
                MessageRecipient = recipients.Select(r => new MessageRecipient()
                {
                    Id_User = Convert.ToInt32(r.Id),
                    Unreaded = true,
                    Visible = true,
                }).ToArray(),
            };

            messageService.SendMessage(entity);

            CreateToastrMessage("Zpráva organizátora byla odeslána");

            return RedirectToAction("Detail", "Akce", new { id });
        }
        #endregion

        #region Photos
        [Authorize]
        public ActionResult Fotky(string id)
        {
            var name = repository.OneByHtml<Event>(id).Name;
            return View(new CreatePhotoAlbum() { Title = "Vložení fotoalba k akci " + name });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken, ActionName("Fotky")]
        public ActionResult FotkyPost(string id, CreatePhotoAlbum album, string submit)
        {
            if (submit == "Zpět na akci")
                return RedirectToAction("Detail", "Akce", new { id });

            var eventEntity = repository.OneByHtml<Event>(id);
            if (!string.IsNullOrWhiteSpace(album.Link))
            {
                try
                {
                    //var separator = new PhotoManager(album.Link, logger);
                    var albumData = rajceService.GetAlbum(album.Link);

                    var albumEntity = new PhotoAlbum()
                    {
                        AlbumLink = album.Link,
                        AspNetUsers = GetUser,
                        CoverPhotoLink = albumData.CoverPhoto.Thumbnail,
                        Discussion = new Discussion()
                        {
                            IsAlone = false,
                            Name = GetUser.UserName + "-" + eventEntity.Name,
                            HtmlName = GetUser.HtmlName + "-" + eventEntity.HtmlName,
                        },
                        Event = eventEntity,
                        PhotoCount = albumData.Photos.Length
                    };

                    repository.Add(albumEntity);

                    logger.LogInfo(string.Format("Uživatel {0} vložil album k akci {1}",
                        GetUser.UserName,
                        eventEntity.Name),
                        "Akce.Fotky");

                    if (!eventEntity.IsPrivate)
                        activityListService.AddActivity(ActivityCreator.Create(GetUser, albumEntity));

                    CreateToastrMessage("Album bylo úspěšně vloženo");

                    // send notice about new album
                    var emailAdressess = eventEntity.EventParticipant
                        .Where(ev => ev.EventLoginStatus == EventLoginEnum.Prijdu)
                        .Select(ep => ep.AspNetUsers)
                        .Distinct()
                        .Where(u => u != GetUser && u.Profiles.SendNewAlbumsToMail)
                        .Select(u => u.Email)
                        .ToArray();

                    emailSender.SendEmail(EmailType.NewPhotoAlbum, albumEntity, emailAdressess);

                    return RedirectToAction("Detail", "Akce", new { id });
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Akce.Fotky");
                    ModelState.AddModelError("", "Nastala neočekávaná chyba");
                }
            }
            else
            {
                ModelState.AddModelError("", "Musí být vložen odkaz");
            }

            return View(new CreatePhotoAlbum() { Title = "Vložení fotoalba k akci " + eventEntity.Name, Link = album.Link });
        }
        #endregion

        #region ICS calendar format
        public ActionResult Calendar(string id)
        {
            var eventEntity = repository.OneByHtml<Event>(id);

            string downloadFileName = "FungujemeAktivne-" + eventEntity.HtmlName;

            var start = TimeZoneInfo.ConvertTimeToUtc(eventEntity.StartDateTime,
                TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
            var stop = TimeZoneInfo.ConvertTimeToUtc(eventEntity.StopDateTime,
                TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR")
                .AppendLine("PRODID:-//FungujemeAktivne//3F")
                .AppendLine("VERSION:2.0")
                .AppendLine("X-MS-OLK-FORCEINSPECTOROPEN:TRUE")
                .AppendLine("BEGIN:VEVENT")
                .AppendLine("DTSTART;TZID=Europe/Prague:" + start.ToString("yyyyMMddTHHmmssZ"))
                .AppendLine("DTEND;TZID=Europe/Prague:" + stop.ToString("yyyyMMddTHHmmssZ"))
                .AppendLine($"LOCATION:{eventEntity.Place}")
                .AppendLine("TRANSP:OPAQUE")
                .AppendLine("SEQUENCE:0")
                .AppendLine($"UID:{downloadFileName}")
                .AppendLine($"DESCRIPTION:{eventEntity.Perex}")
                .Append(@"X-ALT-DESC;FMTTYPE=text/html:<!DOCTYPE HTML PUBLIC "" -//W3C//DTD HTML 3.2//EN"">\n<HTML>\n<HEAD>\n<TITLE></TITLE>\n</HEAD>\n<BODY>")
                .Append(eventEntity.Description)
                .AppendLine(@"</BODY>\n</HTML>")

                .AppendLine($"SUMMARY:{eventEntity.Name}")
                .AppendLine("END:VEVENT")
                .AppendLine("END:VCALENDAR");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            return File(bytes, "text/calendar", downloadFileName + ".ics");
        }
        #endregion

        #region Invitations
        [Authorize]
        public ActionResult Pozvat(string id)
        {
            var eventEntity = repository.OneByHtml<Event>(id);
            if (!eventEntity.IsPrivate || !eventEntity.UserHasInvivation(GetUser))
                return RedirectToHome();

            var model = new EventInvitationsViewModel()
            {
                Users = eventEntity.EventInvitation.Select(s => new User(s.AspNetUsers)).ToList(),
                Icon = "icon-share",
                Title = "Správa pozvánek na akci " + eventEntity.Name,
                UserNames = string.Join(",", eventEntity.EventInvitation.Select(s => s.AspNetUsers.Id)),
                IsOrganisator = eventEntity.EventOrganisator.Any(o => o.AspNetUsers == GetUser),
            };

            model.AddActionButton("Zpět na akci", Utilities.Url("~/Akce/Detail/" + id), "icon-arrow-left");

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Pozvat")]
        public ActionResult PozvatPost(string id, EventInvitationsViewModel model)
        {
            var eventEntity = repository.OneByHtml<Event>(id);
            if (!eventEntity.IsPrivate || !eventEntity.UserHasInvivation(GetUser))
                return RedirectToHome();

            var ids = model.UserNames.Split(',').Select(u => Convert.ToInt32(u)).ToArray();

            // check if organisators are invited
            foreach(var org in eventEntity.EventOrganisator)
            {
                if (!ids.Contains(org.Id_User))
                    ModelState.AddModelError("", $"Organizátor(ka) {org.AspNetUsers.UserName} není pozván(a)");
            }

            if (!ModelState.IsValid)
            {
                model.Icon = "icon-share";
                model.Title = "Správa pozvánek na akci " + eventEntity.Name;
                model.AddActionButton("Zpět na akci", Utilities.Url("~/Akce/Detail/" + id), "icon-arrow-left");
                model.IsOrganisator = eventEntity.EventOrganisator.Any(o => o.AspNetUsers == GetUser);
                model.Users = ids.Select(userId => new User(repository.One<AspNetUsers>(userId))).ToList();

                return View(model);
            }

            var oldInvitations = eventEntity.EventInvitation.Select(ev => ev.AspNetUsers.Id).ToArray();

            // remove invitations
            var removeInvitations = oldInvitations.Except(ids);
            var idsToRemove = eventEntity.EventInvitation
                .Where(s => removeInvitations.Contains(s.Id_User))
                .Select(s => s.Id)
                .ToArray();
            var oldUserNames = new List<string>();

            foreach (var r in idsToRemove)
            {
                var ent = repository.One<EventInvitation>(r);
                oldUserNames.Add(ent.AspNetUsers.UserName);
                repository.Delete(ent);
            }

            logger.LogInfo(string.Format("Uživatel(ka) {0} odebral pozvánky k akci {1} pro tyto uživatele {2}",
                User.Identity.Name,
                eventEntity.HtmlName,
                string.Join(", ", oldUserNames)), "Akce.PozvatPost");

            // new invitations
            var newInvitations = ids.Except(oldInvitations);
            var url = Utilities.Url("~/Akce/Detail/" + eventEntity.HtmlName);
            var newUserNames = new List<string>();
            var message = new Message()
            {
                AspNetUsers = GetSystemUser,
                Id_Sender = GetSystemUser.Id,
                Subject = "Pozvánka na akci " + eventEntity.Name,
                Text = $"Gratuluji\nJsi pozván(a) na soukromou akci <a href='{url}'>{eventEntity.Name}</a>",
                Time = Info.CentralEuropeNow,
                Visible = true,
            };

            foreach(var n in newInvitations)
            {
                eventEntity.EventInvitation.Add(new EventInvitation()
                {
                    Id_User = n,
                });
                message.MessageRecipient.Add(new MessageRecipient()
                {
                    Id_User = n,
                    Unreaded = true,
                    Visible = true,
                });

                newUserNames.Add(repository.One<AspNetUsers>(n).UserName);
            }

            repository.Save();

            messageService.SendMessage(message);

            logger.LogInfo(string.Format("Uživatel(ka) {0} pozval k akci {1} tyto uživatele {2}",
                User.Identity.Name,
                eventEntity.HtmlName,
                string.Join(", ", newUserNames)), "Akce.PozvatPost");

            return RedirectToAction("Detail", "Akce", new { id = eventEntity.HtmlName });
        }
        #endregion
    }
}