using System.Linq;
using System.Web.Mvc;
using _3F.Model.Email;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Model.Service;
using _3F.Web.Definitions;
using _3F.Web.Models;
using _3F.Web.Utils;

namespace _3F.Web.Controllers
{
    public class ZapisController : BaseController
    {
        IActivityListService activityListService;
        IEmailSender emailSender;

        public ZapisController(IActivityListService activityListService, IEmailSender emailSender)
        {
            this.activityListService = activityListService;
            this.emailSender = emailSender;

            Icon = "icon-pencil";
            BackgroundColor = "sea-blue-background";
        }

        public ActionResult Index()
        {
            var model = new EnumerableBaseViewModel<EventSummaryViewModel>()
            {
                Title = "Zápisky z akcí",
                Entities = repository.All<EventSummary>()
                    .OrderByDescending(es => es.Event.StartDateTime)
                    .AsEnumerable()
                    .Select(es => new EventSummaryViewModel()
                    {
                        Perex = es.Perex,
                        Name = es.Name,
                        Author = new User(es.AspNetUsers),
                        HtmlName = es.Event.HtmlName,
                        EventName = es.Event.Name,
                        EventStart = es.Event.StartDateTime.ToDayDateTimeString(),
                        EventStop = es.Event.StopDateTime.ToDayDateTimeString(),
                    }),
            };

            return View(model);
        }

        public ActionResult Detail(string id)
        {
            var summary = repository.One<EventSummary>(es => es.Event.HtmlName == id);
            if (summary == null) return HttpNotFound();

            var model = new EventSummaryViewModel()
            {
                EventName = summary.Event.Name,
                Author = new User(summary.AspNetUsers),
                Description = summary.Description,
                HtmlName = id,
                Id_Discussion = summary.Id_Discussion,
                Name = summary.Name,
                Perex = summary.Perex,
                HasPhoto = summary.Event.PhotoAlbum.Any(),
                Photos = new System.Collections.Generic.List<PhotoAlbumViewModel>(),
            };

            if (model.HasPhoto)
            {
                foreach (var photoAlbum in summary.Event.PhotoAlbum)
                {
                    model.Photos.Add(new PhotoAlbumViewModel(photoAlbum));
                }
            }

            FillSummaryButtons(model);

            if (User.Identity.IsAuthenticated)
                logger.LogInfo(string.Format("Uživatel(ka) {0} navštívil(a) zápisky {1}", User.Identity.Name, model.Name), "Zapis.Detail");

            model.Title = model.Name;

            return View(model);
        }

        [Authorize]
        public ActionResult Vytvorit(string id)
        {
            var eventEntity = repository.One<Event>(ev => ev.HtmlName == id);
            if (eventEntity == null || eventEntity.EventSummary.Count > 0)
                return HttpNotFound(); // zápisky neexistují

            var model = new EventSummaryViewModel()
            {
                HtmlName = id,
                Name = "Zápisky z akce " + eventEntity.Name,
            };

            return View("~/Views/Zapis/_EventSummaryEdit.cshtml", model);
        }

        [Authorize]
        public ActionResult Edit(string id)
        {
            var summary = repository.One<EventSummary>(es => es.Event.HtmlName == id);
            if (summary == null) return HttpNotFound(); // zápisky neexistují

            var model = new EventSummaryViewModel()
            {
                HtmlName = id,
                Name = summary.Name,
                Description = summary.Description,
                Perex = summary.Perex,
            };

            return View("~/Views/Zapis/_EventSummaryEdit.cshtml", model);
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Vytvorit"), ValidateInput(false)]
        public ActionResult VytvoritPost(string id, EventSummaryViewModel model)
        {
            ValidateSummary(model);
            if (ModelState.IsValid)
            {
                var entity = new EventSummary()
                {
                    Description = model.Description,
                    Id_Event = repository.One<Event>(ev => ev.HtmlName == id).Id,
                    Id_User = GetUserId,
                    Name = model.Name,
                    Perex = model.Perex,
                };

                entity.Discussion = new Discussion()
                {
                    Id_Author = GetUserId,
                    IsAlone = false,
                    Name = model.Name,
                    HtmlName = "Zapis_" + id,
                };

                repository.Add(entity);
                CreateToastrMessage("Zápisky byly úspěšně uloženy");
                activityListService.AddActivity(ActivityCreator.Create(GetUser, entity, ActivityType.CreateSummary));
                logger.LogInfo(string.Format("Uživatel {0} vytvořil zápisky {1}", User.Identity.Name, entity.Name), "Zapis.VytvoritPost");

                // send notice about new album
                var eventEntity = repository.One<Event>(ev => ev.HtmlName == id);
                var emailAdressess = eventEntity.EventParticipant
                    .Where(ev => ev.EventLoginStatus == EventLoginEnum.Prijdu)
                    .Select(ep => ep.AspNetUsers)
                    .Distinct()
                    .Where(u => u != GetUser && u.Profiles.SendNewSummaryToMail)
                    .Select(u => u.Email)
                    .ToArray();

                emailSender.SendEmail(EmailType.NewEventSummary, entity, emailAdressess);

                return RedirectToAction("Detail", "Zapis", new { id });
            }
            else
            {
                return View("~/Views/Zapis/_EventSummaryEdit.cshtml", model);
            }
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Edit"), ValidateInput(false)]
        public ActionResult EditPost(string id, EventSummaryViewModel model)
        {
            ValidateSummary(model);
            if (ModelState.IsValid)
            {
                var entity = repository.One<EventSummary>(es => es.Event.HtmlName == id);
                if (entity == null)
                {
                    ModelState.AddModelError("", "Nenalezena odpovídají akce k zápiskům");
                    return View("~/Views/Zapis/_EventSummaryEdit.cshtml", model);
                }

                entity.Description = model.Description;
                entity.Name = model.Name;
                entity.Perex = model.Perex;
                repository.Save();

                CreateToastrMessage("Zápisky byly úspěšně uloženy");
                activityListService.AddActivity(ActivityCreator.Create(GetUser, entity, ActivityType.EditSummary));
                logger.LogInfo(string.Format("Uživatel {0} editoval zápisky {1}", User.Identity.Name, entity.Name), "Zapis.EditPost");
                return RedirectToAction("Detail", "Zapis", new { id });
            }
            else
            {
                return View("~/Views/Zapis/_EventSummaryEdit.cshtml", model);
            }
        }

        private void ValidateSummary(EventSummaryViewModel summary)
        {
            if (string.IsNullOrWhiteSpace(summary.Description))
                ModelState.AddModelError("", "Text zápisků musí být vyplněn.");
            if (string.IsNullOrWhiteSpace(summary.Perex))
                ModelState.AddModelError("", "Krátký popis musí být vyplněn.");
            if (string.IsNullOrWhiteSpace(summary.Name))
                ModelState.AddModelError("", "Název zápisků musí být vyplněn.");
            if (summary.Perex.Length > 300)
                ModelState.AddModelError("", "Krátký popis může mít max. 300 znaků.");
            if (summary.Name.Length > 300)
                ModelState.AddModelError("", "Název zápisků může mít max. 300 znaků.");
        }

        private void FillSummaryButtons(EventSummaryViewModel summary)
        {
            summary.Buttons.Add(new ActionButton("Přejít na akci", Utilities.Url("~/Akce/Detail/", summary.HtmlName, HttpContext), "icon-star"));
            if (User.Identity.IsAuthenticated && (summary.Author.id == GetUserId.ToString() || User.IsInRole(Strings.Administrator)))
            {
                summary.Buttons.Add(new ActionButton("Editovat zápisky", Utilities.Url("~/Zapis/Edit/", summary.HtmlName, HttpContext), "icon-edit"));
            }
        }
    }
}